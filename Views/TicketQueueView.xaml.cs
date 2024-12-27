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
    public TicketQueueView()
    {
        InitializeComponent();
        SortQueue();
    }

    private void SortQueue()
    {
        var view = CollectionViewSource.GetDefaultView(TicketQueueViewModel.TicketQueueTickets);
        view.SortDescriptions.Add(new SortDescription("DateTimeLastUpdated", ListSortDirection.Descending));
        QueueGrid.ItemsSource = view;
    }
    
}

