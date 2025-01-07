using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using TicketeX_.Models;
using TicketeX_.Utilities;
using TicketeX_.Views;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace TicketeX_.ViewModels;

public class ClosedTicketQueueViewModel: ObservableObject
{
    // dodac kolory do wyswietlania ticketow po severity level
    public static ObservableCollection<Ticket> ClosedTickets { get; set; }
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
    
    public ClosedTicketQueueViewModel(ObservableCollection<Ticket> closedTickets, LoggedUser loggedUser)
    {
        ClosedTickets = closedTickets;
        _loggedUser = loggedUser;
        SelectedTicket = ClosedTickets?.FirstOrDefault();
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
        ticketView.ClosedTicketDisplayed();
        ticketView.Show();
    }
    
    
}