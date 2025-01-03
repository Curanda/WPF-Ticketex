using System.Collections.ObjectModel;
using System.Windows.Data;
using TicketeX_.Models;
using TicketeX_.Utilities;
using TicketeX_.Views;

namespace TicketeX_.ViewModels;

public class TicketQueueViewModel: ObservableObject
{
    public static ObservableCollection<Ticket> TicketQueueTickets { get; set; }
   
    public RelayCommand_ ShowTicketCommand { get; set; }
    private LoggedUser _loggedUser;
    private Ticket _selectedTicket { get; set; }

    public Ticket SelectedTicket
    {
        get => _selectedTicket;
        set {
            _selectedTicket = value; 
            OnPropertyChanged();
        }
    }
    
    public TicketQueueViewModel(ObservableCollection<Ticket> ticketQueueTickets, LoggedUser loggedUser)
    {
        TicketQueueTickets = ticketQueueTickets;
        _loggedUser = loggedUser;
        SelectedTicket = TicketQueueTickets?.FirstOrDefault();
        ShowTicketCommand = new RelayCommand_( o =>
        {
            var ticket = _selectedTicket;
            ShowTicketView(ticket);
        });
    }

    private void ShowTicketView(Ticket ticket)
    {
        var ticketVm = new TicketViewModel(ticket, _loggedUser);
        var ticketView = new TicketView
        {
            DataContext = ticketVm
        };
        ticketView.Show();
    }
}