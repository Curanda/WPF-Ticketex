using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

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
        Process.Start("https://slack.com/signin#/signin");
    }

    private void ButtonWorkplace_OnClick(object sender, RoutedEventArgs e)
    {
        Process.Start("https://work.workplace.com/login/input/");
    }

    private void ButtonEmail_OnClick(object sender, RoutedEventArgs e)
    {
        Process.Start("mailto:info@ticketex.dev");
    }

    private void Account_OnClick(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Account");
    }
}
