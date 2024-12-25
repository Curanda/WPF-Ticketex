using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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
    }

    // private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
    // {
    //     var row = sender as DataGridRow;
    //     var selectedTicket = row?.DataContext as Ticket;
    //     var ticketVm = new TicketViewModel(selectedTicket);
    //     var ticketView = new TicketView
    //     {
    //         DataContext = ticketVm
    //     };
    //     ticketView.Show();
    // }
    
}