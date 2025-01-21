using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TicketeX_.ViewModels;

namespace TicketeX_.Views;

public partial class LoginView : Window
{
    public LoginView()
    {
        InitializeComponent();
        Loaded += (s, e) => 
        {
            UsernameBox.Focus();
        };
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        var passwordBox = sender as PasswordBox;
        var password = passwordBox.Password;
        if (DataContext is not LoginViewModel loginVm) return;
        loginVm.isPasswordSet = password.Trim().Length != 0;
        PasswordErrorPopup.IsOpen = password.Trim().Length == 0;
    }

    private void AcceptsReturnToLogin(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            LoginButton.Command.Execute(LoginButton.CommandParameter);
        }
    }
}