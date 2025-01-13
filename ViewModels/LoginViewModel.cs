using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Controls;
using System.Windows;
using Dapper;
using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using TicketeX_.Models;
using TicketeX_.Utilities;
using TicketeX_.Views;

namespace TicketeX_.ViewModels;

public class LoginViewModel: ObservableObject, INotifyDataErrorInfo
{
    private string _username;
    public bool isPasswordSet { get; set; }
    public RelayCommand_ LoginCommand { get; set; }

    private readonly Dictionary<string, List<string>> _validationMessages = new Dictionary<string, List<string>>();
    public bool HasErrors => _validationMessages.Any();
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            ClearErrors(Username);
            if (string.IsNullOrWhiteSpace(value)) AddError(nameof(Username), "User name field cannot be empty");
            OnPropertyChanged();
        }
    }

    public LoginViewModel()
    {
        LoginCommand = new RelayCommand_(parameter =>
        {
            
            var passwordBox = parameter as PasswordBox;
            if (passwordBox.Password.Length == 0)
            {
                isPasswordSet = true;
                return;
            }
            LoginUser(_username, parameter);
            if (parameter is IDisposable disposable)
            {
                disposable.Dispose();
                passwordBox.Password = string.Empty;
            }
        });
        
    }
    
    public IEnumerable GetErrors(string propertyName)
    {
        return _validationMessages.ContainsKey(propertyName) ? _validationMessages[propertyName] : null;
    }

    private void AddError(string propertyName, string validationMessage )
    {
        if (!_validationMessages.ContainsKey(propertyName)) _validationMessages[propertyName] = new List<string>();
        if (!_validationMessages.ContainsKey(validationMessage)) _validationMessages[propertyName].Add(validationMessage);
        OnErrorsChanged(propertyName);
    }

    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    private void ClearErrors(string propertyName)
    {
        if (_validationMessages.ContainsKey(propertyName))
        {
            _validationMessages.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }
    }

    private async Task LoginUser(string _username, object parameter)
    {
        await using var connection = new MySqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        await connection.OpenAsync();
        var passwordBox = parameter as PasswordBox;
        string query = "SELECT UserId, FirstName, LastName, Department FROM all_users WHERE UserId = @UserId AND Password = @Password";
    
        try
        {
            var res = connection.Query<LoggedUser>(query, new 
            { 
                UserId = _username, 
                Password = passwordBox?.Password
            }).FirstOrDefault();
            

            if (res == null)
                MessageBox.Show("Invalid username or password");
            else
            {
                LoggedUser loggedUser = new LoggedUser
                {
                    FirstName = res.FirstName,
                    LastName = res.LastName,
                    Department = res.Department,
                    UserId = res.UserId
                };

                MainViewModel mainViewModel = new MainViewModel(loggedUser);
                MainWindow mainView = new MainWindow
                {
                    DataContext = mainViewModel
                };
                mainView.Show();
                
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is not MainWindow)
                    {
                        window.Close();
                    }
                }
            }
            passwordBox?.Clear();
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }



}