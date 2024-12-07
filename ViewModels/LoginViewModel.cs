using System.Configuration;
using System.Windows.Controls;
using System.Windows;
using Dapper;
using MySql.Data.MySqlClient;
using TicketeX_.Models;
using TicketeX_.Utilities;
using TicketeX_.Views;

namespace TicketeX_.ViewModels;

public class LoginViewModel: ObservableObject
{
    private string _username;
    public RelayCommand_ LoginCommand { get; set; }

    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged();
        }
    }

    public LoginViewModel()
    {
        LoginCommand = new RelayCommand_(parameter =>
        {
            
            LoginUser(_username, parameter);
            if (parameter is IDisposable disposable)
            {
                disposable.Dispose();
            }
        });
        
    }

    private async Task LoginUser(string _username, object parameter)
    {
        Console.WriteLine(_username);
        await using var connection = new MySqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        await connection.OpenAsync();
        var passwordBox = parameter as PasswordBox;
        string query = "SELECT UserId, FirstName, LastName, Department FROM all_users WHERE UserId = @UserId AND Password = @Password";
    
        try
        {
            Console.WriteLine("trying connection to db");
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

/*
 * TODO
 * zapelnic baze userami
 * zrobic autentykacje z baza w vw
 * pokazywac kolejke zgodnie z dzialem usera
 * podpisywac tickety zgodnie z dzialem i loginem usera
 * zrobic logout i powrot do screena login
 *
 *
 * wystylizowac datagrid
 * p-klik na ticket i otworz ticketa oraz kopiuj ticket.
 * edycja, delegowanie i zamykanie ticketa.
 *
 *
 * migracja na postgres i listen/notify dla odswiezania kolejki.
 *
 * kolejka alertow: llm i generacja losowych alertow.
 * p-klik myszy kopiowanie alertow i zamykanie alertow.
 * 
 */