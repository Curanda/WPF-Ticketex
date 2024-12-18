using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Google.Protobuf.WellKnownTypes;
using MaterialDesignThemes.Wpf;
using TicketeX_.Models;
using TicketeX_.Utilities;
using TicketeX_.Views;

namespace TicketeX_.ViewModels;

public class TicketQueueView_Model: ObservableObject, INotifyPropertyChanged
{
    public ObservableCollection<Ticket> TicketQueueTickets { get; set; }
    public RelayCommand_ ShowTicketCommand { get; set; }
    public TicketQueueView_ ticketQueueView_ { get; set; }
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
    public TicketQueueView_Model(ObservableCollection<Ticket> ticketQueueTickets, LoggedUser loggedUser)
    {
        TicketQueueTickets = ticketQueueTickets;
        _loggedUser = loggedUser;
        SelectedTicket = TicketQueueTickets?.FirstOrDefault();
        ShowTicketCommand = new RelayCommand_( _selectedTicket =>
        {
            var ticket = _selectedTicket as Ticket;
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