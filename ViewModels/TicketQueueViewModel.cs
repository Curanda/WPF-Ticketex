using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using TicketeX_.Models;
using TicketeX_.Utilities;

namespace TicketeX_.ViewModels;

public class TicketQueueViewModel: ObservableObject
{
    public ObservableCollection<TicketQueueTicket>? HelpdeskTickets { get; set; }
    public TicketQueueTicket? SelectedTicket { get; set; }
    public TicketQueueViewModel(ObservableCollection<TicketQueueTicket>? helpdeskTickets)
    {
        HelpdeskTickets = helpdeskTickets;
        SelectedTicket = helpdeskTickets?.FirstOrDefault();
        wyswietlSelected();
    }

    private void wyswietlSelected()
    {
        while (true)
        {
            if (SelectedTicket == null) return;
            Console.WriteLine(SelectedTicket.Description);
            Thread.Sleep(5000);
        }
    }
    
    
    
}