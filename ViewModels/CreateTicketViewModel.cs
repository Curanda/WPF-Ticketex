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
    private bool _validatedDestination { get; set; }
    public List<string> Severities { get; } = ["Low", "Medium", "High", "Critical"];
    public List<string> Destinations { get; } = ["Helpdesk", "Windowssupport", "Networksupport", "Hr", "Lager", "Dbsupport", "Technician", "Janitor"];
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

    public bool ValidatedDestination
    {
        get => _validatedDestination;
        set
        {
            _validatedDestination = value;
            OnPropertyChanged(nameof(ValidatedDestination));
        }
    }

    public string SelectedSeverity
    {
        get => _selectedSeverity;
        set
        {
            _selectedSeverity = value;
            Console.WriteLine("selected severity is : " + SelectedSeverity + " Value is: " +value);
            ValidatedSeverity = false;
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

    public string SelectedDestination
    {
        get => _selectedDestination;
        set
        {
            _selectedDestination = value;
            Console.WriteLine("selected destination is : " + SelectedDestination + " Value is: " +value);
            ValidatedDestination = false;
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
      _validatedDestination = false;
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
        ValidatedDestination = SelectedDestination.IsNullOrEmpty();
        Console.WriteLine($"validated severity {ValidatedSeverity}");
        
        return !string.IsNullOrWhiteSpace(_reportedBy) 
               && _reportedBy.Contains('@')
               && !string.IsNullOrWhiteSpace(_selectedSeverity)
               && !string.IsNullOrWhiteSpace(_selectedDestination)
               && !string.IsNullOrWhiteSpace(_description);
    }

    private async Task CreateTicket()
    {
        var ticket = new Ticket
            {
            TicketId = null,
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
        var isSuccess = await DatabaseEngine.CreateTicket(ticket);
        if (isSuccess) MessageBox.Show($"A new ticket with Id: {ticket.TicketId}, has been created successfully and sent to {ticket.CurrentLocation}");
        ClearFormFields();
        ShowCreatedTicket(ticket);
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