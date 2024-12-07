using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using Dapper;
using MySql.Data.MySqlClient;
using TicketeX_.CustomAttributes;
using TicketeX_.Models;
using TicketeX_.Utilities;
using Enum = System.Enum;

namespace TicketeX_.ViewModels;

public class CreateTicketViewModel : ObservableObject
{
    public RelayCommand_ CreateTicketCommand { get; }
    private LoggedUser _loggedUser;
    private Severity _selectedSeverity;
    private Severity _severity;
    private Location _selectedDestination;
    private Location _location;
    private string _author;
    private string _reportedBy;
    private string _description;
    private string _connectionString = ConfigurationManager.AppSettings["ConnectionString"];

    public ComboBoxItem SelectedSeverity
    {
        set
        {
            CastSeverityToEnum(value.Content.ToString());
            _selectedSeverity = _severity;
            OnPropertyChanged();
        }
    }

    public string Author
    {
        get => _author; 
        set
        {
            _author = value;
            OnPropertyChanged();
        }
    }

    public string ReportedBy
    {
        get => _reportedBy;
        set
        {
            _reportedBy = value;
            OnPropertyChanged();
        }
    }
    
    private void CastSeverityToEnum(string targetString)
    {
        foreach (Severity item in Enum.GetValues(typeof(Severity)))
        {
            if (targetString.Equals(item.ToString()))
            {
                _severity = item;
                break;
            }
        }
    }
    
    private void CastDestinationToEnum(string targetString)
    {
        foreach (Location item in Enum.GetValues(typeof(Location)))
        {
            if (targetString.Equals(item.ToString()))
            {
                _location = item;
                break;
            }
        }
    }

    public ComboBoxItem SelectedDestination
    {
        set
        {
            CastDestinationToEnum(value.Content.ToString());
            _selectedDestination = _location;
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
    public CreateTicketViewModel(LoggedUser loggedUser)
    {
      _loggedUser = loggedUser;
      _author = loggedUser.UserId;
      CreateTicketCommand = new RelayCommand_(async (parameter)=>
      {
          await CreateTicket(parameter);
      });
    }

    private async Task<string> GenerateNewTicketNumber()
    {
        var targetDepartment = EnumDescription.GetDescription(_loggedUser.Department);
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        string query = @"SELECT TotalTickets FROM ticket_counter WHERE Department = @targetDepartment FOR UPDATE";
        
        try
        {
            Console.WriteLine("trying connection to db");
            var numOfTickets = connection.Query<int>(query, new { targetDepartment }).FirstOrDefault();
            Console.WriteLine($"numOfTickets: {numOfTickets}");
            int newTicketNumber = numOfTickets + 1;
            string newTicketId = $"{targetDepartment}-{newTicketNumber}";
            string updateQuery = "UPDATE ticket_counter SET TotalTickets = @newTicketNumber WHERE Department = @targetDepartment";
            var isSuccess = await connection.ExecuteAsync(updateQuery, new { newTicketNumber, targetDepartment });

            Console.WriteLine($"isSuccess: {isSuccess}");
            return newTicketId;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
            return "MySQL Error";
        }
    }

    private async Task CreateTicket(object parameter)
    {
        string newTicketId = await GenerateNewTicketNumber();
        Console.WriteLine($"new ticket number: {newTicketId}");
        var ticket = new Ticket_
        (
            newTicketId,
            _loggedUser.Department,
            Status.Open,
            _selectedSeverity,
            _loggedUser.UserId,
            _selectedDestination,
            _loggedUser.Department,
            _reportedBy,
            DateTime.Now,
            DateTime.Now,
            _description,
            0,
            0,
            0,
            0,
            0
        );
        try
        {
            Console.WriteLine($"ticket created: {ticket.Status}, {ticket.Description}, {ticket.DateTimeCreated}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        await SendTicketToDb(ticket);
    }
    
    private async Task SendTicketToDb(Ticket_ ticket)
    {
        try
        {
            await using var connection = new MySqlConnection(_connectionString);
            var query = $"INSERT INTO {_selectedDestination.ToString().ToLower()}_tickets (TicketId, Status, Severity, AuthorId, Origin, CurrentLocation, PrevLocation, ReporterId, Description, NumOfUpdates, NumOfUpVotes, NumOfDownVotes, VotesRatio, Attachments) VALUES (@TicketId, @Status, @Severity, @AuthorId, @Origin, @CurrentLocation, @PrevLocation, @ReporterId, @Description, @NumOfUpdates, @NumOfUpVotes, @NumOfDownVotes, @VotesRatio, @Attachments)";
            await connection.ExecuteAsync(query, new
            {
                ticket.TicketId,
                Status = ticket.Status.ToString(),
                Severity = ticket.Severity.ToString(),
                ticket.AuthorId,
                Origin = ticket.Origin.ToString(),
                CurrentLocation = ticket.CurrentLocation.ToString(),
                PrevLocation = ticket.PrevLocation.ToString(),
                ticket.ReporterId,
                ticket.Description,
                ticket.NumOfUpdates,
                ticket.NumOfUpVotes,
                ticket.NumOfDownVotes,
                ticket.VotesRatio,
                ticket.Attachments
            });
            MessageBox.Show($"A new ticket with Id: {ticket.TicketId}, has been created successfully and sent to {ticket.CurrentLocation}");
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
}