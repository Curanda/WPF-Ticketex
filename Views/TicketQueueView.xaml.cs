using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TicketeX_.Models;
using TicketeX_.Utilities;
using TicketeX_.ViewModels;
using DataGridRow = System.Windows.Controls.DataGridRow;

namespace TicketeX_.Views;

public partial class TicketQueueView
{
    private CollectionViewSource TicketsViewSource { get; } = new CollectionViewSource { Source = TicketQueueViewModel.TicketQueueTickets };
    public TicketQueueView()
    {
        InitializeComponent();
        DefaultSortQueue();
    }

    private void DefaultSortQueue()
    {
        TicketsViewSource.SortDescriptions.Add(new SortDescription("DateTimeLastUpdated", ListSortDirection.Descending));
        QueueGrid.ItemsSource = TicketsViewSource.View;
    }
    
}

