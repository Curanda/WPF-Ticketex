using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using TicketeX_.Models;
using TicketeX_.Utilities;

namespace TicketeX_.ViewModels;

public class TicketViewModel: ObservableObject
{
    private bool ticketDelegated = false;
    private Ticket ticket;
    private Ticket uneditedTicket;
    private LoggedUser loggedUser;
    public string TicketId { get; set; }
    public string _status { get; set; }
    public string _severity { get; set; }
    public string AuthorId { get; set; }
    public string Origin { get; set; }
    public string _currentLocation { get; set; }
    public string _prevLocation { get; set; }
    public string ReporterId { get; set; }
    public DateTime DateTimeCreated { get; set; }
    public DateTime _dateTimeLastUpdated { get; set; }
    private string _selectedSeverity { get; set; }
    public RelayCommand_ EditButton_Click { get; }
    public RelayCommand_ SaveButton_Click { get; }
    public static RelayCommand_ CloseTicket_Click { get; set; }
    public RelayCommand_ CancelButton_Click { get; }
    private string _selectedDestination { get; set; }
    private string _description { get; set; }
    private string OldDescription { get; set; }
    public List<string> Severities { get; } = ["Low", "Medium", "High", "Critical"];
    public List<string> Destinations { get; } = ["Helpdesk", "Windowssupport", "Networksupport", "Hr", "Lager", "Dbsupport", "Technician", "Janitor"];

    public string Status
    {
        get => ticket.Status;
        set
        {
        ticket.Status = value;
        OnPropertyChanged();
        }
    }
    
    public DateTime DateTimeLastUpdated
    {
        get => ticket.DateTimeLastUpdated;
        set => ticket.DateTimeLastUpdated = value;
    }

    public ComboBoxItem Severity
    {
        set
        {
            _severity = value.Content.ToString();
            OnPropertyChanged();
        }
    }
    

    public ComboBoxItem SelectedDestination
    {
        set
        {
            _selectedDestination = value.Content.ToString();
            OnPropertyChanged();
        }
    }

    public string Description
    {
        get => ticket.Description;
        set
        {
            _description = value;
            OnPropertyChanged();
        }
    }

    public TicketViewModel(Ticket selectedTicket, LoggedUser loggedUser)
    {
        ticket = selectedTicket;
        uneditedTicket = selectedTicket.ShallowCopy();
        this.loggedUser = loggedUser;
        TicketId = selectedTicket.TicketId;
        _severity = selectedTicket.Severity;
        _status = selectedTicket.Status;
        AuthorId = selectedTicket.AuthorId;
        Origin = selectedTicket.Origin;
        DateTimeCreated = selectedTicket.DateTimeCreated;
        _dateTimeLastUpdated = selectedTicket.DateTimeLastUpdated;
        _description = selectedTicket.Description;
        _prevLocation = selectedTicket.PrevLocation;
        ReporterId = selectedTicket.ReporterId;
        _selectedDestination = null;
        OldDescription = selectedTicket.Description;
        
        // EditButton_Click = new RelayCommand_(o =>
        // {
        //     Console.WriteLine("Edit button clicked");
        // });
        
        SaveButton_Click = new RelayCommand_(async void (o) =>
        {
            if (uneditedTicket.Status == "Closed")
            {
                ticket.Status = "Open";
                ticket.Description = ticket.Description+$"\n \n {ticket.DateTimeLastUpdated}, Ticket reopened by: {this.loggedUser.UserId}, Department: {this.loggedUser.Department} \n \n Reason for reopening: "+_description;
                var isSuccess = await DatabaseEngine.ReopenTicket(ticket);
                if (isSuccess) MessageBox.Show($"Ticket number {ticket.TicketId} reopened successfully and sent to {ticket.CurrentLocation}");
                if (ClosedTicketQueueViewModel.ClosedTickets.Contains(ticket))
                {
                    ClosedTicketQueueViewModel.ClosedTickets.Remove(ticket);
                }
                return;
            }
            
            ticket.Status = "Pending";
            ticket.DateTimeLastUpdated = DateTime.Now;
            ticket.NumOfUpdates++;
            if ((_selectedDestination == null) || (_selectedDestination == loggedUser.Department))
            {
                ticket.Description = ticket.Description+$"\n \n {ticket.DateTimeLastUpdated}, Updated by: {this.loggedUser.UserId}, Department: {this.loggedUser.Department} \n \n Update: "+_description;
                var isSuccess = await DatabaseEngine.UpdateTicket(ticket);
                if (isSuccess) MessageBox.Show($"Ticket number {ticket.TicketId} updated successfully in {ticket.CurrentLocation} queue");
                if (!isSuccess) MessageBox.Show($"Ticket number {ticket.TicketId} update failure");
            }
            else
            {
                 ticket.CurrentLocation = _selectedDestination;
                 ticket.Description = ticket.Description+$"\n \n {ticket.DateTimeLastUpdated}, Delegated by: {this.loggedUser.UserId}, Department: {this.loggedUser.Department} \n \n Update: "+_description;
                 var isSuccess = await DatabaseEngine.TransferTicket(ticket);
                 if (isSuccess)
                 {
                     MessageBox.Show(
                         $"Ticket number {ticket.TicketId} successfully transferred to {ticket.CurrentLocation} queue");
                     if (TicketQueueViewModel.TicketQueueTickets.Contains(ticket))
                     {
                         TicketQueueViewModel.TicketQueueTickets.Remove(ticket);
                     }
                 }
            }
            // MainViewModel.RefreshOpenTicketsCommand.Execute(null);
            
        });

        CloseTicket_Click = new RelayCommand_(async void (o) =>
        {
            var willTicketBeClosed = MessageBox.Show("Are you sure You want to close this ticket?",
                $"Closing ticket no {ticket.TicketId}", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, MessageBoxResult.OK);
            if (willTicketBeClosed == MessageBoxResult.OK) await CloseTicket();
            
        });
        
        CancelButton_Click = new RelayCommand_(o =>
        {
            ticket.Severity = uneditedTicket.Severity;
            ticket.Status = uneditedTicket.Status;
            Description = uneditedTicket.Description;
            _selectedDestination = null;
        });
    }

    private async Task<bool> CloseTicket()
    {
        ticket.Status = "Closed";
        ticket.DateTimeLastUpdated = DateTime.Now;
        ticket.Description = ticket.Description+$"\n \n {ticket.DateTimeLastUpdated}, Ticket CLOSED by user: {loggedUser.UserId}, Department: {this.loggedUser.Department} \n \n Reason for closure: "+_description;
        var isSuccess = await DatabaseEngine.CloseTicket(ticket);
        if (isSuccess)
        {
            MessageBox.Show($"Ticket number {ticket.TicketId} closed successfully");
            if (TicketQueueViewModel.TicketQueueTickets.Contains(ticket))
            {
                TicketQueueViewModel.TicketQueueTickets.Remove(ticket);
            }
        };
        return isSuccess;
    }
    
}