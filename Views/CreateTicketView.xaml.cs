using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mysqlx;
using Regex = System.Text.RegularExpressions.Regex;

namespace TicketeX_.Views;

public partial class CreateTicketView : UserControl
{
    public CreateTicketView()
    {
        InitializeComponent();
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e) 
    {
        SeverityComboBox.SelectedItem = null;
        DestinationComboBox.SelectedItem = null;
        ReportedByTextBox.Text = string.Empty;
        DescriptionTextBox.Text = string.Empty;
    }
}