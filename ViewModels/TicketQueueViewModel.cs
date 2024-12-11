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

public class TicketQueueViewModel: ObservableObject, INotifyPropertyChanged
{
    public ObservableCollection<Ticket> TicketQueueTickets { get; set; }
    public RelayCommand_ ShowTicketCommand { get; set; }
    public TicketQueueView ticketQueueView { get; set; }
    private Ticket _selectedTicket { get; set; }

    public Ticket SelectedTicket
    {
        get => _selectedTicket;
        set {
            _selectedTicket = value; OnPropertyChanged();
        }
    }
    public TicketQueueViewModel(ObservableCollection<Ticket> ticketQueueTickets)
    {
        TicketQueueTickets = ticketQueueTickets;
        SelectedTicket = TicketQueueTickets?.FirstOrDefault();
        ShowTicketCommand = new RelayCommand_( _selectedTicket =>
        {
            var ticket = _selectedTicket as Ticket;
            Console.WriteLine("siemkla");
            Console.WriteLine(ticket?.TicketId);
            Show(ticket);
        });
    }

    private void Show(Ticket ticket)
    {
        var ticketVm = new TicketViewModel(ticket);
        var ticketView = new TicketView
        {
            DataContext = ticketVm
        };
        ticketView.Show();
    }
}