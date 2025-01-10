using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Messaging;
using TicketeX_.ViewModels;

namespace TicketeX_.Views;

public partial class TicketView : Window
{
    private bool checkTicketClosed;

    public TicketView(string selectedTicketStatus)
    {
        InitializeComponent();
        checkTicketClosed = selectedTicketStatus == "Closed";
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
        if (checkTicketClosed)
        {
            ReopenTicketButton.Visibility = Visibility.Visible;
            EditButton.Visibility = Visibility.Collapsed;
        }
        else
        {
            ReopenTicketButton.Visibility = Visibility.Collapsed;
            EditButton.Visibility = Visibility.Visible;
        }
        
        CloseTicketButton.Visibility = Visibility.Collapsed;
        SaveButton.Visibility = Visibility.Collapsed;
        CancelButton.Visibility = Visibility.Collapsed;
        CloseButton.Visibility = Visibility.Visible;
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
        var willTicketBeClosed = MessageBox.Show("Are you sure You want to close this ticket?", $"Closing ticket no XXXX", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, MessageBoxResult.OK);
        if (willTicketBeClosed == MessageBoxResult.OK) TicketViewModel.CloseTicket.Execute(null);
        Close();
        StrongReferenceMessenger.Default.Send("closed_ticket_queue_refresh_required");
    }

    private void ReopenTicketButton_Click(object sender, RoutedEventArgs e)
    {
        EditButton.Visibility = Visibility.Collapsed;
        ReopenTicketButton.Visibility = Visibility.Collapsed;
        SaveButton.Visibility = Visibility.Visible;
        CancelButton.Visibility = Visibility.Visible;
        CloseButton.Visibility = Visibility.Collapsed;
        EnableFormEditing(true);
    }
}