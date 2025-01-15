using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using Dapper;
using MySql.Data.MySqlClient;
using TicketeX_.Models;
using TicketeX_.Utilities;
using TicketeX_.CustomAttributes;
using ComboBox = System.Windows.Controls.ComboBox;

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
    
    private readonly string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
    
    private string _selectedSeverity { get; set; }
    public RelayCommand_ EditButton_Click { get; }
    public RelayCommand_ SaveButton_Click { get; }
    public static RelayCommand_ CloseTicket { get; set; }
    public RelayCommand_ CancelButton_Click { get; }
    private string _selectedDestination { get; set; }
    private string _description { get; set; }
    private string OldDescription { get; set; }

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
            ticket.Description = value;
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
        
        EditButton_Click = new RelayCommand_(o =>
        {
            Console.WriteLine("Edit button clicked");
        });
        
        SaveButton_Click = new RelayCommand_(o =>
        {
            var ticketReopening = ticket.Status == "Closed";
            ticket.Status = "Pending";
            ticket.DateTimeLastUpdated = DateTime.Now;
            // OldDescription = _ticket.Description;
            ticket.Description = ticket.Description+$"\n \n {ticket.DateTimeLastUpdated}, Update by: {this.loggedUser.UserId}, Department: {this.loggedUser.Department} \n \n "+_description;
            // Description = ticket.Description;
            // OldDescription = uneditedTicket.Description;
            ticket.NumOfUpdates++;
            if (_selectedDestination != null) ticket.CurrentLocation = _selectedDestination;
            if ((_selectedDestination != this.loggedUser.Department) && (_selectedDestination != null)) ticket.PrevLocation = this.loggedUser.Department;
            
            if (ticketReopening) DatabaseEngine.ReopenTicket(ticket);
            
            
            // DelegateTicket();
            if (uneditedTicket.Status != "Closed") MainViewModel.RefreshQueueCommand.Execute(null);
            if (uneditedTicket.Status == "Closed") MainViewModel.RefreshClosedTicketsCommand.Execute(null);
        });

        CloseTicket = new RelayCommand_(async void (o) =>
        {
            ticket.Status = "Closed";
            await DelegateTicket();
            MainViewModel.RefreshClosedTicketsCommand.Execute(null);
            MainViewModel.RefreshQueueCommand.Execute(null);
        });
        
        CancelButton_Click = new RelayCommand_(o =>
        {
            _severity = uneditedTicket.Severity;
            _status = uneditedTicket.Status;
            Description = uneditedTicket.Description;
            _selectedDestination = null;
        });
    }

    private async Task DelegateTicket()
    {
        try
        {
            await using var connection = new MySqlConnection(connectionString);
            string query;
            
            if ((_selectedDestination == loggedUser.Department) || (_selectedDestination == null))
            {
                query = uneditedTicket.Status != "Closed" 
                    ? $"UPDATE {loggedUser.Department.ToLower()}_tickets SET Status = @Status, Severity = @Severity, Description = @Description, DateTimeLastUpdated = @DateTimeLastUpdated WHERE TicketId = @TicketId" 
                    : $"INSERT INTO {loggedUser.Department.ToLower()}_tickets (TicketId, Status, Severity, AuthorId, Origin, CurrentLocation, PrevLocation, ReporterId, Description, DateTimeCreated, DateTimeLastUpdated, NumOfUpdates, NumOfUpVotes, NumOfDownVotes, VotesRatio, Attachments) VALUES (@TicketId, @Status, @Severity, @AuthorId, @Origin, @CurrentLocation, @PrevLocation, @ReporterId, @Description, @DateTimeCreated, @DateTimeLastUpdated, @NumOfUpdates, @NumOfUpVotes, @NumOfDownVotes, @VotesRatio, @Attachments)";
            }
            else
            {
                query = $"INSERT INTO {_selectedDestination.ToLower()}_tickets (TicketId, Status, Severity, AuthorId, Origin, CurrentLocation, PrevLocation, ReporterId, Description, DateTimeCreated, DateTimeLastUpdated, NumOfUpdates, NumOfUpVotes, NumOfDownVotes, VotesRatio, Attachments) VALUES (@TicketId, @Status, @Severity, @AuthorId, @Origin, @CurrentLocation, @PrevLocation, @ReporterId, @Description, @DateTimeCreated, @DateTimeLastUpdated, @NumOfUpdates, @NumOfUpVotes, @NumOfDownVotes, @VotesRatio, @Attachments)";
            }
            
            if (ticket.Status == "Closed")
            {
                query = $"INSERT INTO closed_tickets (TicketId, Status, Severity, AuthorId, Origin, CurrentLocation, PrevLocation, ReporterId, Description, DateTimeCreated, DateTimeLastUpdated, NumOfUpdates, NumOfUpVotes, NumOfDownVotes, VotesRatio, Attachments) VALUES (@TicketId, @Status, @Severity, @AuthorId, @Origin, @CurrentLocation, @PrevLocation, @ReporterId, @Description, @DateTimeCreated, @DateTimeLastUpdated, @NumOfUpdates, @NumOfUpVotes, @NumOfDownVotes, @VotesRatio, @Attachments)";
            }
            
            await connection.ExecuteAsync(query, new
            {
                ticket.TicketId,
                ticket.Status,
                ticket.Severity,
                ticket.AuthorId,
                ticket.Origin,
                ticket.CurrentLocation,
                ticket.PrevLocation,
                ticket.ReporterId,
                ticket.Description,
                ticket.DateTimeCreated,
                ticket.DateTimeLastUpdated,
                ticket.NumOfUpdates,
                ticket.NumOfUpVotes,
                ticket.NumOfDownVotes,
                ticket.VotesRatio,
                ticket.Attachments
            });
            

            if (uneditedTicket.Status != "Closed")
            {
                if ((_selectedDestination != loggedUser.Department) && (_selectedDestination != null))
                {
                    try
                    {
                        var deleteQuery = $"DELETE FROM {loggedUser.Department.ToLower()}_tickets WHERE TicketId = @TicketId";
                        await connection.ExecuteAsync(deleteQuery, new { ticket.TicketId });
                        if (ticket.Status != "Closed") MessageBox.Show($"Ticket with Id: {ticket.TicketId}, has been successfully delegated to {ticket.CurrentLocation}. It's Someone else's problem now");
                        if (ticket.Status == "Closed") MessageBox.Show($"Ticket with id of : '{ticket.TicketId}' has been closed. Congratulations!!!");
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("Unable to delete ticket");
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    if (ticket.Status != "Closed") MessageBox.Show($"Ticket with Id: {ticket.TicketId}, has been successfully updated");
                    if (ticket.Status == "Closed") MessageBox.Show($"Ticket with id of : '{ticket.TicketId}' has been closed. Congratulations!!!");
                }
                
                
            } else
            {
                try
                {
                    var deleteQuery = $"DELETE FROM closed_tickets WHERE TicketId = @TicketId";
                    await connection.ExecuteAsync(deleteQuery, new { ticket.TicketId });
                    MessageBox.Show($"Ticket with Id: {ticket.TicketId}, has been successfully reopened and sent to {ticket.CurrentLocation}.");
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Unable to delete ticket");
                    Console.WriteLine(ex.Message);
                }
            }

        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Unable to delegate ticket");
            Console.WriteLine(ex.Message);
        }
    }
    
    
}