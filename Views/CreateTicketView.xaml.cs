using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mysqlx;
using Regex = System.Text.RegularExpressions.Regex;

namespace TicketeX_.Views;

public partial class CreateTicketView : UserControl
{
    public CreateTicketView()
    {
        InitializeComponent();
        
        StrongReferenceMessenger.Default.Register<string>(this, (r, m) =>
        {
            if (m == "CreateTicketView_ClearFormFields") ClearFormFields();
        });
    }

    private void ClearFormFields() 
    {
        SeverityComboBox.SelectedItem = null;
        DestinationComboBox.SelectedItem = null;
        ReportedByTextBox.Text = string.Empty;
        DescriptionTextBox.Text = string.Empty;
    }
}