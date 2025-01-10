using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Messaging;
using TicketeX_.Models;
using TicketeX_.Utilities;
using TicketeX_.ViewModels;

namespace TicketeX_.Views;

public partial class ClosedTicketQueueView : UserControl
{
    private bool SevLo_Checked, SevHi_Checked, SevMed_Checked, SevCrit_Checked;
    private CollectionViewSource ClosedTicketsViewSource { get; } = new CollectionViewSource { Source = ClosedTicketQueueViewModel.ClosedTickets };
    
    
    
    public ClosedTicketQueueView()
    {
        InitializeComponent();
        DefaultSortQueue();
        QueueGrid.ItemsSource = ClosedTicketsViewSource.View;
        SevLo_Checked = true;
        SevHi_Checked  = true;
        SevMed_Checked = true;
        SevCrit_Checked = true;
        SetFilter();
        StrongReferenceMessenger.Default.Register<string>(this, (r, m) =>
        {
            if (m == "closed_ticket_queue_refresh_required") ClosedTicketsViewSource.View.Refresh();
        });
    }
    
    private void DefaultSortQueue()
    {
        ClosedTicketsViewSource.SortDescriptions.Add(new SortDescription("DateTimeLastUpdated", ListSortDirection.Descending));
    }

    private void SortSevLoFirst_OnClick(object sender, RoutedEventArgs e)
    {
        ClosedTicketsViewSource.SortDescriptions.Clear();
        ClosedTicketsViewSource.SortDescriptions.Add(new SortDescription("SeverityNumber", ListSortDirection.Ascending));
    }

    private void SortSevHiFirst_OnClick(object sender, RoutedEventArgs e)
    {
        ClosedTicketsViewSource.SortDescriptions.Clear();
        ClosedTicketsViewSource.SortDescriptions.Add(new SortDescription("SeverityNumber", ListSortDirection.Descending));
    }

    private void SortLastUpdatedAsc_OnClick(object sender, RoutedEventArgs routedEventArgs)
    {
        ClosedTicketsViewSource.SortDescriptions.Clear();
        ClosedTicketsViewSource.SortDescriptions.Add(new SortDescription("DateTimeLastUpdated", ListSortDirection.Ascending));
    }

    private void SortLastUpdatedDesc_OnClick(object sender, RoutedEventArgs routedEventArgs)
    {
        ClosedTicketsViewSource.SortDescriptions.Clear();
        ClosedTicketsViewSource.SortDescriptions.Add(new SortDescription("DateTimeLastUpdated", ListSortDirection.Descending));
    }
    
    public void SevLo(object sender, RoutedEventArgs routedEventArgs)
    {
        SevLo_Checked = !SevLo_Checked;
        ClosedTicketsViewSource.View.Refresh();
    }

    public void SevHi(object sender, RoutedEventArgs routedEventArgs)
    {
        SevHi_Checked = !SevHi_Checked;
        ClosedTicketsViewSource.View.Refresh();
    }

    public void SevMed(object sender, RoutedEventArgs routedEventArgs)
    {
        SevMed_Checked = !SevMed_Checked;
        ClosedTicketsViewSource.View.Refresh();
    }

    public void SevCrit(object sender, RoutedEventArgs routedEventArgs)
    {
        SevCrit_Checked = !SevCrit_Checked;
        ClosedTicketsViewSource.View.Refresh();
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
    
    private void TicketsViewSource_Filter(object sender, FilterEventArgs e)
    {
        if (e.Item is not Ticket ticket) return;
         
        e.Accepted = (SevLo_Checked && ticket?.Severity == "Low") ||
                      (SevMed_Checked && ticket?.Severity == "Medium") ||
                      (SevHi_Checked && ticket?.Severity == "High") ||
                      (SevCrit_Checked && ticket?.Severity == "Critical");
    }

    private void SetFilter()
    {
        ClosedTicketsViewSource.Filter += new FilterEventHandler(TicketsViewSource_Filter);
        ClosedTicketsViewSource.View.Refresh();
    }

    private void ResetFilters()
    {
        SevLo_Checked = true;
        FilterCheckSevLo.IsChecked = true;
        SevHi_Checked  = true;
        FilterCheckSevHi.IsChecked = true;
        SevMed_Checked = true;
        FilterCheckSevMed.IsChecked = true;
        SevCrit_Checked = true;
        FilterCheckSevCrit.IsChecked = true;
    }
    
    private void RefreshFiltersButton_OnClick(object sender, RoutedEventArgs e)
    {
        ResetFilters();
        ResetFilters();
        ClosedTicketsViewSource.SortDescriptions.Clear();
        DefaultSortQueue();
        ClosedTicketsViewSource.View.Refresh();
    }
}