using System.Windows;

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
        StatusComboBox.IsEnabled = enable;
        SeverityComboBox.IsEnabled = enable;
        DescriptionTextBox.IsEnabled = enable;
        OriginTextBox.IsEnabled = enable;
        CurrentLocationTextBox.IsEnabled = enable;
        PrevLocationTextBox.IsEnabled = enable;
    }
}