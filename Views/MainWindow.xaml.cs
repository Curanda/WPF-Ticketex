using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using TicketeX_.ViewModels;

namespace TicketeX_.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void AccountButton_OnClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var contextMenu = button?.ContextMenu;
        if (contextMenu == null) return;
        contextMenu.DataContext = button.DataContext;
        contextMenu.IsOpen = true;
        e.Handled = true;
    }

    private void ButtonSlack_OnClick(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://slack.com/signin#/signin",
            UseShellExecute = true
        });
    }

    private void ButtonWorkplace_OnClick(object sender, RoutedEventArgs e)
    { ;
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://work.workplace.com/login/input/",
            UseShellExecute = true
        });
    }

    private void ButtonEmail_OnClick(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "mailto:info@ticketex.dev",
            UseShellExecute = true
        });
    }

    private void Profile_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel mainViewModel) return;
        var accountInfoView = new AccountInfoView(mainViewModel.LoggedUser);
        accountInfoView.ShowDialog();
    }
}
