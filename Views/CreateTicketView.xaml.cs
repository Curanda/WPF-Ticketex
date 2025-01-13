using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using TicketeX_.ViewModels;

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
        if (!SeverityDropdownCollection.Contains(SeverityPlaceholder))
            SeverityDropdownCollection.Insert(0, SeverityPlaceholder);
    
        if (!DestinationDropdownCollection.Contains(DestinationPlaceholder))
            DestinationDropdownCollection.Insert(0, DestinationPlaceholder);
        SeverityComboBox.SelectedIndex = 0;
        DestinationComboBox.SelectedIndex = 0;
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

    private void DestinationComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is CreateTicketViewModel createTicketVm)
        {
            createTicketVm.SelectedDestination = e.AddedItems[0] as string;
        }
    }

    private void DestinationComboBox_OnDropDownOpened(object sender, EventArgs e)
    {
        DestinationComboBox.SelectedIndex = 1;
        DestinationDropdownCollection.Remove(DestinationPlaceholder);
    }
}