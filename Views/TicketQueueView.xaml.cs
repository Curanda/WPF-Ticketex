using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TicketeX_.Models;
using TicketeX_.ViewModels;
using SortDescription = System.ComponentModel.SortDescription;

namespace TicketeX_.Views;

public partial class TicketQueueView
{
    private CollectionViewSource TicketsViewSource { get; } = new CollectionViewSource { Source = TicketQueueViewModel.TicketQueueTickets };
    private bool SevLo_Checked, SevHi_Checked, SevMed_Checked, SevCrit_Checked;


    public TicketQueueView()
    {
        InitializeComponent();
        DefaultSortQueue();
        QueueGrid.ItemsSource = TicketsViewSource.View;
        SevLo_Checked = true;
        SevHi_Checked  = true;
        SevMed_Checked = true;
        SevCrit_Checked = true;
        SetFilter();
    }

    private void DefaultSortQueue()
    {
        TicketsViewSource.SortDescriptions.Add(new SortDescription("DateTimeLastUpdated", ListSortDirection.Descending));
    }

    private void OpenContextMenuOnLeftClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var contextMenu = button?.ContextMenu;
        if (contextMenu == null) return;
        contextMenu.DataContext = button.DataContext;
        contextMenu.IsOpen = true;
        e.Handled = true;
    }

    private void SortSevLoFirst_OnClick(object sender, RoutedEventArgs e)
    {
        TicketsViewSource.SortDescriptions.Clear();
        
    }

    private void SortSevHiFirst_OnClick(object sender, RoutedEventArgs e)
    {
        TicketsViewSource.SortDescriptions.Clear();
        
    }

    private void SortLastUpdatedAsc_OnClick(object sender, RoutedEventArgs routedEventArgs)
    {
        TicketsViewSource.SortDescriptions.Clear();
        TicketsViewSource.SortDescriptions.Add(new SortDescription("DateTimeLastUpdated", ListSortDirection.Ascending));
    }

    private void SortLastUpdatedDesc_OnClick(object sender, RoutedEventArgs routedEventArgs)
    {
        TicketsViewSource.SortDescriptions.Clear();
        TicketsViewSource.SortDescriptions.Add(new SortDescription("DateTimeLastUpdated", ListSortDirection.Descending));
    }
    
    public void SevLo(object sender, RoutedEventArgs routedEventArgs)
    {
        SevLo_Checked = !SevLo_Checked;
        TicketsViewSource.View.Refresh();
    }

    public void SevHi(object sender, RoutedEventArgs routedEventArgs)
    {
        SevHi_Checked = !SevHi_Checked;
        TicketsViewSource.View.Refresh();
    }

    public void SevMed(object sender, RoutedEventArgs routedEventArgs)
    {
        SevMed_Checked = !SevMed_Checked;
        TicketsViewSource.View.Refresh();
    }

    public void SevCrit(object sender, RoutedEventArgs routedEventArgs)
    {
        SevCrit_Checked = !SevCrit_Checked;
        TicketsViewSource.View.Refresh();
    }

    public void TicketsViewSource_Filter(object sender, FilterEventArgs e)
    {
        var ticket = e.Item as Ticket;
        e.Accepted = (SevLo_Checked && ticket.Severity == "Low") ||
                     (SevMed_Checked && ticket.Severity == "Medium") ||
                     (SevHi_Checked && ticket.Severity == "High") ||
                     (SevCrit_Checked && ticket.Severity == "Critical");
    }

    private void SetFilter()
    {
        TicketsViewSource.Filter += new FilterEventHandler(TicketsViewSource_Filter);
        TicketsViewSource.View.Refresh();
    }
    
}

