using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TicketeX_.ViewModels;
using SortDescription = System.ComponentModel.SortDescription;

namespace TicketeX_.Views;

public partial class TicketQueueView
{
    private CollectionViewSource TicketsViewSource { get; } = new CollectionViewSource { Source = TicketQueueViewModel.TicketQueueTickets };
    public TicketQueueView()
    {
        InitializeComponent();
        DefaultSortQueue();
        QueueGrid.ItemsSource = TicketsViewSource.View;
    }

    private void DefaultSortQueue()
    {
        TicketsViewSource.SortDescriptions.Add(new SortDescription("DateTimeLastUpdated", ListSortDirection.Descending));
    }
    
    private void Bold_Checked(object sender, RoutedEventArgs e)
    {
        FilterMenuButton.FontWeight = FontWeights.Bold;
    }

    private void Bold_Unchecked(object sender, RoutedEventArgs e)
    {
        FilterMenuButton.FontWeight = FontWeights.Normal;
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
        
    }

    private void SortSevHiFirst_OnClick(object sender, RoutedEventArgs e)
    {

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
}

