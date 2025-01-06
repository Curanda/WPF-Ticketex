using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Messaging;
using TicketeX_.Models;
using TicketeX_.Utilities;
using TicketeX_.ViewModels;
using SortDescription = System.ComponentModel.SortDescription;

namespace TicketeX_.Views;

public partial class TicketQueueView
{
    private CollectionViewSource TicketsViewSource { get; } = new CollectionViewSource { Source = TicketQueueViewModel.TicketQueueTickets };
    private bool SevLo_Checked, SevHi_Checked, SevMed_Checked, SevCrit_Checked;
    private IMessenger _messenger;

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
        
        StrongReferenceMessenger.Default.Register<StatusMessage>(this,(r,m)=>
        {
            if (m.Status != "queue_refreshed") return;
            // ResetFilters();
        });
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

    private void TicketsViewSource_Filter(object sender, FilterEventArgs e)
    {
        var ticket = e.Item as Ticket;
        e.Accepted = (SevLo_Checked && ticket?.Severity == "Low") ||
                     (SevMed_Checked && ticket?.Severity == "Medium") ||
                     (SevHi_Checked && ticket?.Severity == "High") ||
                     (SevCrit_Checked && ticket?.Severity == "Critical");
    }

    private void SetFilter()
    {
        TicketsViewSource.Filter += new FilterEventHandler(TicketsViewSource_Filter);
        TicketsViewSource.View.Refresh();
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
        // Dlaczego wywołuję to dwa razy? Filtr nie aktualizuje view po programatycznej zmianie wartości. View zostaje
        // odświeżone dopiero przy drugim wywołaniu...
        ResetFilters();
        ResetFilters();
        TicketsViewSource.View.Refresh();
    }
}

