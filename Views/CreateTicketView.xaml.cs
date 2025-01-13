using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mysqlx;
using TicketeX_.ViewModels;
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

    private void SeverityComboBox_OnDropDownOpened(object sender, EventArgs e)
    {
        SeverityComboBox.SelectedIndex = 1;
        SeverityDropdownCollection.Remove(SeverityPlaceholder);
    }

    private void SeverityComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is CreateTicketViewModel createTicketVm)
        {
            createTicketVm.SelectedSeverity = e.AddedItems[0] as string;
        }
    }
}