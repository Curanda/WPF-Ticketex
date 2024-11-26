using System.Windows;
using TicketeX_.ViewModels;

namespace TicketeX_.Views;

public partial class LoginView : Window
{
    public LoginView()
    {
        InitializeComponent();
        if (DataContext is LoginViewModel loginVm)
        {
            loginVm.CloseAction = Close;
        }
    }
}