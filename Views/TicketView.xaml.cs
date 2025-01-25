using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Messaging;
using TicketeX_.ViewModels;

namespace TicketeX_.Views;

public partial class TicketView : Window
{
    private bool isTicketClosed;

    public TicketView(string selectedTicketStatus)
    {
        InitializeComponent();
        isTicketClosed = selectedTicketStatus == "Closed";
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
        if (isTicketClosed)
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
        DestinationComboBox.SelectedIndex = 0;
        EnableFormEditing(false);
    }
    
    public void EnableFormEditing(bool enable)
    {
        SeverityComboBox.IsEnabled = enable;
        DescriptionTextBox.IsEnabled = enable;
        DestinationComboBox.IsEnabled = enable;
        if (enable)
        {
            var editBinding = new Binding("Description")
            {
                Mode = BindingMode.OneWayToSource
            };
            DescriptionTextBox.SetBinding(TextBox.TextProperty, editBinding);
        }
        else
        {
            var displayBinging = new Binding("Description")
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
        Close();
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

    private void DestinationComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is TicketViewModel ticketVm)
        {
            ticketVm.SelectedDestination = e.AddedItems[0] as string;
        }
    }

    private void DestinationComboBox_OnDropDownOpened(object sender, EventArgs e)
    {
        DestinationComboBox.SelectedIndex = 1;
        DestinationDropdownCollection.Remove(DestinationPlaceholder);
    }
}