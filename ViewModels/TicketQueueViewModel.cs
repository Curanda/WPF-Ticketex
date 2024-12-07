using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using TicketeX_.Models;
using TicketeX_.Utilities;

namespace TicketeX_.ViewModels;

public class TicketQueueViewModel: ObservableObject
{
    public ObservableCollection<TicketQueueTicket_> TicketQueueTickets { get; set; }
    public TicketQueueTicket_ SelectedTicket { get; set; }
    public TicketQueueViewModel(ObservableCollection<TicketQueueTicket_> ticketQueueTickets)
    {
        TicketQueueTickets = ticketQueueTickets;
        // SelectedTicket = TicketQueueTickets?.FirstOrDefault();
        // DisplaySelected();
    }

    private void DisplaySelected()
    {
        // while (true)
        // {
        //     if (SelectedTicket == null) return;
        //     Console.WriteLine(SelectedTicket?.Description);
        //     Thread.Sleep(5000);
        // }
    }
}