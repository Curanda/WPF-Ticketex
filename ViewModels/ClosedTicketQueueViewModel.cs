using System.Collections.ObjectModel;
using System.Windows;
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
    public RelayCommand_ CopyTicketDataCommand { get; set; }
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
        
        CopyTicketDataCommand = new RelayCommand_(o =>
        {
            var ticket = _selectedTicket;
            CopyTicketDetailsToClipboard(ticket);
        });
    }

    private void ShowTicketView(Ticket ticket)
    {
        var ticketVm = new TicketViewModel(ticket, _loggedUser);
        var ticketView = new TicketView(SelectedTicket.Status)
        {
            DataContext = ticketVm
        };
        ticketView.ClosedTicketDisplayed();
        ticketView.Show();
    }
    
    private void CopyTicketDetailsToClipboard(Ticket ticket)
    {
        Clipboard.Clear();
        Clipboard.SetText($"Ticket: {ticket.TicketId}, status: {ticket.Status}, created: {ticket.DateTimeCreated} by {ticket.AuthorId}.\n Description: {ticket.Description}");
    }
    
}