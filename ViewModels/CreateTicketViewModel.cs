using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using TicketeX_.Models;
using TicketeX_.Utilities;
using TicketeX_.Views;

namespace TicketeX_.ViewModels;

public class CreateTicketViewModel : ObservableObject, INotifyDataErrorInfo
{
    public RelayCommand_ CreateTicketCommand { get; }
    private readonly LoggedUser _loggedUser;
    private string _selectedSeverity;
    private string _selectedDestination;
    private string _reportedBy;
    private string _description;
    private bool _validatedSeverity { get; set; }
    public List<string> Severities { get; } = ["Low", "Medium", "High", "Critical"];
    public string SeverityPlaceholder { get; } = "Please select a severity.";
    private readonly string _connectionString = ConfigurationManager.AppSettings["ConnectionString"];
    private readonly Dictionary<string, List<string>> _validationMessages = new Dictionary<string, List<string>>();
    public bool HasErrors => _validationMessages.Any();
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    
    public bool ValidatedSeverity
    {
        get => _validatedSeverity;
        set
        {
            _validatedSeverity = value;
            OnPropertyChanged(nameof(ValidatedSeverity));
        }
    }

    public string SelectedSeverity
    {
        get => _selectedSeverity;
        set
        {
            _selectedSeverity = value;
            Console.WriteLine("selected severity is : " + SelectedSeverity + " Value is: " +value);
            _validatedSeverity = false;
            OnPropertyChanged();
        }
    }
    

    public string ReportedBy
    {
        get => _reportedBy;
        set
        {
            ClearErrors(nameof(ReportedBy));
            _reportedBy = value;
            if (string.IsNullOrWhiteSpace(value)) 
                AddError(nameof(ReportedBy), "Reported By cannot be empty");
            else if (!value.Contains('@'))
                AddError(nameof(ReportedBy), "Reported By has to be an email address");
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
            ClearErrors(nameof(Description));
            _description = value;
            if (string.IsNullOrWhiteSpace(value)) 
                AddError(nameof(Description), "Description cannot be empty. Provide an update");
            OnPropertyChanged();
        }
    }
    public CreateTicketViewModel(LoggedUser loggedUser)
    {
      _loggedUser = loggedUser;
      _validatedSeverity = false;
      CreateTicketCommand = new RelayCommand_( (o)=>
      {
          if (FinalValidation())
          { 
              Console.WriteLine($"final validation says : {FinalValidation()}");
              ClearErrors(nameof(ReportedBy));
              ClearErrors(nameof(Description));
              Console.WriteLine($"Reported By: {ReportedBy}");
              Console.WriteLine($"Description: {Description}");
              foreach (var validationMessage in _validationMessages) Console.WriteLine(validationMessage);
              CreateTicket();
          }
          else
          {
              MessageBox.Show($"You've got errors in your form");
              foreach (var validationMessage in _validationMessages) Console.WriteLine(validationMessage);
          }
      });
    }
    
    public IEnumerable GetErrors(string propertyName)
    {
        return _validationMessages.ContainsKey(propertyName) ? _validationMessages[propertyName] : null;
    }
    
    private void AddError(string propertyName, string validationMessage )
    {
        if (!_validationMessages.ContainsKey(propertyName)) _validationMessages[propertyName] = new List<string>();
        if (!_validationMessages[propertyName].Contains(validationMessage)) _validationMessages[propertyName].Add(validationMessage);
        OnErrorsChanged(propertyName);
    }

    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    private void ClearErrors(string propertyName)
    {
        if (_validationMessages.ContainsKey(propertyName))
        {
            _validationMessages.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }
    }

    private bool FinalValidation()
    {
        Console.WriteLine("reportedBy is null of whitespace: "+_reportedBy);
        Console.WriteLine("selected severity is "+_selectedSeverity);
        Console.WriteLine("description is "+Description);
        ValidatedSeverity = SelectedSeverity.IsNullOrEmpty();
        Console.WriteLine($"validated severity {ValidatedSeverity}");
        
        return !string.IsNullOrWhiteSpace(_reportedBy) 
               && _reportedBy.Contains('@')
               && !string.IsNullOrWhiteSpace(_selectedSeverity)
               && !string.IsNullOrWhiteSpace(_description);
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
            ClearFormFields();
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
        var createdTicketView = new TicketView(ticket.Status)
        {
            DataContext = createdTicketVm
        };
        if (!string.Equals(_loggedUser.Department, _selectedDestination, StringComparison.CurrentCultureIgnoreCase)) ProhibitCreatedTicketEditing(createdTicketView);
        createdTicketView.Show();
        ClearErrors(nameof(ReportedBy));
        ClearErrors(nameof(Description));
        ClearErrors(nameof(SelectedSeverity));
    }

    private void ProhibitCreatedTicketEditing(TicketView createdTicketView)
    {
        createdTicketView.EditButton.Visibility = Visibility.Collapsed;
        createdTicketView.SaveButton.Visibility = Visibility.Collapsed;
        createdTicketView.CancelButton.Visibility = Visibility.Collapsed;
        createdTicketView.CloseButton.Visibility = Visibility.Visible;
        createdTicketView.EnableFormEditing(false);
    }

    private void ClearFormFields()
    {
        StrongReferenceMessenger.Default.Send<string>("CreateTicketView_ClearFormFields");
    }
}