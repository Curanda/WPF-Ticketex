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
    private Ticket _ticket;
    private Ticket uneditedTicket;
    private LoggedUser _loggedUser;
    
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
    public RelayCommand_ CancelButton_Click { get; }
    private string _selectedDestination { get; set; }
    private string _description { get; set; }
    private string OldDescription { get; set; }

    public string Status
    {
        get => _ticket.Status;
        set
        {
        _ticket.Status = value;
        OnPropertyChanged();
        }
    }
    
    // public string PrevLocation { get => _prevLocation; }

    public DateTime DateTimeLastUpdated
    {
        get => _ticket.DateTimeLastUpdated;
        set => _ticket.DateTimeLastUpdated = value;
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
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged();
        }
    }

    public TicketViewModel(Ticket selectedTicket, LoggedUser loggedUser)
    {
        _ticket = selectedTicket;
        uneditedTicket = selectedTicket;
        _loggedUser = loggedUser;
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
        Console.WriteLine(uneditedTicket.CurrentLocation);
        
        EditButton_Click = new RelayCommand_(o =>
        {
            Console.WriteLine("Edit button clicked");
        });
        
        SaveButton_Click = new RelayCommand_(o =>
        {
            Console.WriteLine("Save button clicked");
            _ticket.Status = "Pending";
            _ticket.DateTimeLastUpdated = DateTime.Now;
            _ticket.DateTimeCreated = uneditedTicket.DateTimeCreated;
            OldDescription = _ticket.Description;
            _ticket.Description = OldDescription+$"\n \n {_ticket.DateTimeLastUpdated}, Update by: {_loggedUser.UserId}, Department: {_loggedUser.Department} \n \n "+_description;
            Description = _ticket.Description;
            OldDescription = _ticket.Description;
            if (_selectedDestination != null) _ticket.CurrentLocation = _selectedDestination;
            if ((_selectedDestination != _loggedUser.Department) && (_selectedDestination != null)) _ticket.PrevLocation = _loggedUser.Department;
            _ticket.NumOfUpdates++;
            Console.WriteLine("_selected: "+_selectedDestination);
            Console.WriteLine("unedited: "+_loggedUser.Department);
            DelegateTicket();
            MainViewModel.RefreshQueueCommand.Execute(null);
        });
        
        CancelButton_Click = new RelayCommand_(o =>
        {
            Console.WriteLine("Cancel button clicked");
            _severity = uneditedTicket.Severity;
            Status = uneditedTicket.Status;
            Description = uneditedTicket.Description;

            SelectedDestination = null;
        });
    }

    private async Task DelegateTicket()
    {
        
        try
        {
            await using var connection = new MySqlConnection(connectionString);
            string query;
            if ((_selectedDestination == _loggedUser.Department) || (_selectedDestination == null))
            {
                if (_selectedDestination == null) Console.WriteLine("_selectedDestination is null");
                if (_selectedDestination == _loggedUser.Department) Console.WriteLine($"_selectedDestination is equal to {uneditedTicket.CurrentLocation}. ticket.CurrentLocation is {_ticket.CurrentLocation}");
                query = $"UPDATE {_loggedUser.Department.ToLower()}_tickets SET Status = @Status, Severity = @Severity, Description = @Description WHERE TicketId = @TicketId";
            }
            else
            {
                Console.WriteLine($"Selected destination is a new value of {_selectedDestination}");
                query = $"INSERT INTO {_selectedDestination.ToLower()}_tickets (TicketId, Status, Severity, AuthorId, Origin, CurrentLocation, PrevLocation, ReporterId, Description, NumOfUpdates, NumOfUpVotes, NumOfDownVotes, VotesRatio, Attachments) VALUES (@TicketId, @Status, @Severity, @AuthorId, @Origin, @CurrentLocation, @PrevLocation, @ReporterId, @Description, @NumOfUpdates, @NumOfUpVotes, @NumOfDownVotes, @VotesRatio, @Attachments)";
            }
            await connection.ExecuteAsync(query, new
            {
                _ticket.TicketId,
                _ticket.Status,
                _ticket.Severity,
                _ticket.AuthorId,
                _ticket.Origin,
                _ticket.CurrentLocation,
                _ticket.PrevLocation,
                _ticket.ReporterId,
                _ticket.Description,
                _ticket.NumOfUpdates,
                _ticket.NumOfUpVotes,
                _ticket.NumOfDownVotes,
                _ticket.VotesRatio,
                _ticket.Attachments
            });

            if ((_selectedDestination != _loggedUser.Department) && (_selectedDestination != null))
            {
                try
                {
                    var deleteQuery = $"DELETE FROM {_loggedUser.Department.ToLower()}_tickets WHERE TicketId = @TicketId";
                    await connection.ExecuteAsync(deleteQuery, new { _ticket.TicketId });
                    MessageBox.Show($"Ticket with Id: {_ticket.TicketId}, has been successfully delegated to {_ticket.CurrentLocation}. It's Someone else's problem now");
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Unable to delete ticket");
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                MessageBox.Show($"Ticket with Id: {_ticket.TicketId}, has been successfully updated");
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Unable to delegate ticket");
            Console.WriteLine(ex.Message);
        }
    }
    
    
}