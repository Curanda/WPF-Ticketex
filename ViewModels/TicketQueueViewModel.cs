using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Messaging;
using TicketeX_.Models;
using TicketeX_.Utilities;
using TicketeX_.Views;

namespace TicketeX_.ViewModels;

public class TicketQueueViewModel: ObservableObject
{
    public static ObservableCollection<Ticket> TicketQueueTickets { get; set; }
   
    public RelayCommand_ ShowTicketCommand { get; set; }
    public RelayCommand_ CopyTicketDataCommand { get; set; }
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
        ticketView.Show();
    }

    private void CopyTicketDetailsToClipboard(Ticket ticket)
    {
        Clipboard.Clear();
        Clipboard.SetText($"Ticket: {ticket.TicketId}, status: {ticket.Status}, created: {ticket.DateTimeCreated} by {ticket.AuthorId}.\n Description: {ticket.Description}");
    }
}

