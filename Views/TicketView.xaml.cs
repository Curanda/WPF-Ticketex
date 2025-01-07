using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TicketeX_.Views;

public partial class TicketView : Window
{
    public TicketView()
    {
        InitializeComponent();
    }

    public void ClosedTicketDisplayed()
    {
        ReopenTicketButton.Visibility = Visibility.Visible;
        CloseTicketButton.Visibility = Visibility.Collapsed;
        CloseButton.Visibility = Visibility.Visible;
        EditButton.Visibility = Visibility.Collapsed;
        SaveButton.Visibility = Visibility.Collapsed;
        CancelButton.Visibility = Visibility.Collapsed;
        EnableFormEditing(false);
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        ReopenTicketButton.Visibility = Visibility.Collapsed;
        CloseTicketButton.Visibility = Visibility.Visible;
        EditButton.Visibility = Visibility.Collapsed;
        SaveButton.Visibility = Visibility.Visible;
        CancelButton.Visibility = Visibility.Visible;
        EnableFormEditing(true);
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        ReopenTicketButton.Visibility = Visibility.Collapsed;
        CloseTicketButton.Visibility = Visibility.Collapsed;
        CloseButton.Visibility = Visibility.Visible;
        EditButton.Visibility = Visibility.Collapsed;
        SaveButton.Visibility = Visibility.Collapsed;
        CancelButton.Visibility = Visibility.Collapsed;
        EnableFormEditing(false);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        ReopenTicketButton.Visibility = Visibility.Collapsed;
        CloseTicketButton.Visibility = Visibility.Collapsed;
        EditButton.Visibility = Visibility.Visible;
        SaveButton.Visibility = Visibility.Collapsed;
        CancelButton.Visibility = Visibility.Collapsed;
        EnableFormEditing(false);
    }
    
    public void EnableFormEditing(bool enable)
    {
        SeverityComboBox.IsEnabled = enable;
        DescriptionTextBox.IsEnabled = enable;
        DestinationComboBox.IsEnabled = enable;
        if (enable)
        {
            Binding editBinding = new Binding("Description")
            {
                Mode = BindingMode.OneWayToSource
            };
            DescriptionTextBox.SetBinding(TextBox.TextProperty, editBinding);
        }
        else
        {
            Binding displayBinging = new Binding("Description")
            {
                Mode = BindingMode.OneWay
            };
            DescriptionTextBox.SetBinding(TextBox.TextProperty, displayBinging);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void CloseTicketButton_Click(object sender, RoutedEventArgs e)
    {
        var willTicketBeClosed = MessageBox.Show("Are you sure You want to close this ticket?", "closing", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, MessageBoxResult.OK);
        if (willTicketBeClosed == MessageBoxResult.OK) Close();
    }

    private void ReopenTicketButton_Click(object sender, RoutedEventArgs e)
    {
        EditButton.Visibility = Visibility.Collapsed;
        SaveButton.Visibility = Visibility.Visible;
        CancelButton.Visibility = Visibility.Visible;
        EnableFormEditing(true);
    }
}