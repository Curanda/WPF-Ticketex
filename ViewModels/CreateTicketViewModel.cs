using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using Dapper;
using MySql.Data.MySqlClient;
using TicketeX_.Models;
using TicketeX_.Utilities;
using TicketeX_.Views;

namespace TicketeX_.ViewModels;

public class CreateTicketViewModel : ObservableObject
{
    public RelayCommand_ CreateTicketCommand { get; }
    private readonly LoggedUser _loggedUser;
    private string _selectedSeverity;
    private string _selectedDestination;
    private string _reportedBy;
    private string _description;
    private readonly string _connectionString = ConfigurationManager.AppSettings["ConnectionString"];

    public ComboBoxItem SelectedSeverity
    {
        set
        {
            _selectedSeverity = value.Content.ToString();
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
    public CreateTicketViewModel(LoggedUser loggedUser)
    {
      _loggedUser = loggedUser;
      CreateTicketCommand = new RelayCommand_( (o)=>
      {
          CreateTicket();
      });
    }

    private async Task<string> GenerateNewTicketNumber()
    {
        string targetDepartment = _loggedUser.Department;
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        string query = @"SELECT TotalTickets FROM ticket_counter WHERE Department = @targetDepartment FOR UPDATE";
        
        try
        {
            int newTicketNumber = connection.Query<int>(query, new { targetDepartment }).FirstOrDefault()+1;
            string newTicketId = $"{targetDepartment}-{newTicketNumber}";
            string updateQuery = "UPDATE ticket_counter SET TotalTickets = @newTicketNumber WHERE Department = @targetDepartment";
            int isSuccess = await connection.ExecuteAsync(updateQuery, new { newTicketNumber, targetDepartment });

            Console.WriteLine($"isSuccess: {isSuccess}");
            return newTicketId;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
            return "MySQL Error";
        }
    }

    private async Task CreateTicket()
    {
        string newTicketId = await GenerateNewTicketNumber();
        Console.WriteLine($"new ticket number: {newTicketId}");
        var ticket = new Ticket
            {
            TicketId = newTicketId,
            Origin = _loggedUser.Department,
            Status = "Open",
            Severity = _selectedSeverity,
            AuthorId = _loggedUser.UserId,
            CurrentLocation = _selectedDestination,
            PrevLocation = _loggedUser.Department,
            ReporterId = ReportedBy,
            DateTimeCreated = DateTime.Now,
            DateTimeLastUpdated = DateTime.Now,
            Description = Description,
            NumOfUpdates = 0,
            NumOfDownVotes = 0,
            NumOfUpVotes = 0,
            VotesRatio = 0,
            Attachments = 0
        };
        await SendTicketToDb(ticket);
    }
    
    private async Task SendTicketToDb(Ticket ticket)
    {
        try
        {
            await using var connection = new MySqlConnection(_connectionString);
            var query = $"INSERT INTO {_selectedDestination.ToLower()}_tickets (TicketId, Status, Severity, AuthorId, Origin, CurrentLocation, PrevLocation, ReporterId, Description, NumOfUpdates, NumOfUpVotes, NumOfDownVotes, VotesRatio, Attachments) VALUES (@TicketId, @Status, @Severity, @AuthorId, @Origin, @CurrentLocation, @PrevLocation, @ReporterId, @Description, @NumOfUpdates, @NumOfUpVotes, @NumOfDownVotes, @VotesRatio, @Attachments)";
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
                ticket.NumOfUpdates,
                ticket.NumOfUpVotes,
                ticket.NumOfDownVotes,
                ticket.VotesRatio,
                ticket.Attachments
            });
            MessageBox.Show($"A new ticket with Id: {ticket.TicketId}, has been created successfully and sent to {ticket.CurrentLocation}");
            ShowCreatedTicket(ticket);
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void ShowCreatedTicket(Ticket ticket)
    {
        var createdTicketVm = new TicketViewModel(ticket, _loggedUser);
        var createdTicketView = new TicketView
        {
            DataContext = createdTicketVm
        };
        if (!string.Equals(_loggedUser.Department, _selectedDestination, StringComparison.CurrentCultureIgnoreCase)) ProhibitCreatedTicketEditing(createdTicketView);
        createdTicketView.Show();
    }

    private void ProhibitCreatedTicketEditing(TicketView createdTicketView)
    {
        createdTicketView.EditButton.Visibility = Visibility.Collapsed;
        createdTicketView.SaveButton.Visibility = Visibility.Collapsed;
        createdTicketView.CancelButton.Visibility = Visibility.Collapsed;
        createdTicketView.CloseButton.Visibility = Visibility.Visible;
        createdTicketView.EnableFormEditing(false);
    }
}