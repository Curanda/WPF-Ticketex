using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;
using Org.BouncyCastle.Crypto.Engines;
using TicketeX_.Models;
using TicketeX_.ViewModels;

namespace TicketeX_.Views;

public partial class TicketView : Window
{
    public TicketView()
    {
        InitializeComponent();
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        EditButton.Visibility = Visibility.Collapsed;
        SaveButton.Visibility = Visibility.Visible;
        CancelButton.Visibility = Visibility.Visible;
        EnableFormEditing(true);
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        EditButton.Visibility = Visibility.Visible;
        SaveButton.Visibility = Visibility.Collapsed;
        CancelButton.Visibility = Visibility.Collapsed;
        EnableFormEditing(false);
        // var vm = (TicketViewModel)DataContext;
        // vm.editCounter++;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        EditButton.Visibility = Visibility.Visible;
        SaveButton.Visibility = Visibility.Collapsed;
        CancelButton.Visibility = Visibility.Collapsed;
        EnableFormEditing(false);
    }
    
    private void EnableFormEditing(bool enable)
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
        throw new NotImplementedException();
    }
}