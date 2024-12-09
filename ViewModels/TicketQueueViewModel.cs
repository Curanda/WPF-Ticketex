using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using TicketeX_.Models;
using TicketeX_.Utilities;

namespace TicketeX_.ViewModels;

public class TicketQueueViewModel: ObservableObject
{
    public ObservableCollection<Ticket> TicketQueueTickets { get; set; }
    public Ticket SelectedTicket { get; set; }
    public TicketQueueViewModel(ObservableCollection<Ticket> ticketQueueTickets)
    {
        TicketQueueTickets = ticketQueueTickets;
        SelectedTicket = TicketQueueTickets?.FirstOrDefault();
        // DisplaySelected();
    }

    private void DisplaySelected()
    {
        // while (SelectedTicket != null)
        // {
        //     Console.WriteLine(SelectedTicket?.Description);
        //     Thread.Sleep(5000);
        // }
    }
}