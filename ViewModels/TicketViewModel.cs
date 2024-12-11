using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using TicketeX_.Models;
using TicketeX_.Utilities;
using TicketeX_.CustomAttributes;
using ComboBox = System.Windows.Controls.ComboBox;

namespace TicketeX_.ViewModels;

public class TicketViewModel: ObservableObject
{
    private Ticket ticket;
    public string EditorId = "Editor";
    public string EditorDepartmentId = "EditorDepartmentId";
    
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
    
    private readonly string _connectionString = ConfigurationManager.AppSettings["ConnectionString"];
    
    private string _selectedSeverity { get; set; }
    public RelayCommand_ EditButton_Click { get; }
    public RelayCommand_ SaveButton_Click { get; }
    public RelayCommand_ CancelButton_Click { get; }
    private string _selectedDestination { get; set; }
    private string _description { get; set; }
    private string OldDescription { get; set; }

    public string Status
    {
        get => _status;
        set
        {
        _status = value;
        OnPropertyChanged();
        }
    }
    
    public string PrevLocation { get => _prevLocation; set => _currentLocation = value; }

    public DateTime DateTimeLastUpdated
    {
        get => _dateTimeLastUpdated;
        set => _dateTimeLastUpdated = value;
    }

    public string Severity
    {
        get => _severity;
        set
        {
            _severity = value;
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
        }
    }
    

    public TicketViewModel(Ticket selectedTicket)
    {
        ticket = selectedTicket;
        TicketId = selectedTicket.TicketId;
        Severity = selectedTicket.Severity;
        Status = selectedTicket.Status;
        AuthorId = selectedTicket.AuthorId;
        Origin = selectedTicket.Origin;
        DateTimeCreated = selectedTicket.DateTimeCreated;
        _dateTimeLastUpdated = selectedTicket.DateTimeLastUpdated;
        _description = selectedTicket.Description;
        _prevLocation = selectedTicket.PrevLocation;
        ReporterId = selectedTicket.ReporterId;
        
        OldDescription = selectedTicket.Description;
        Description = selectedTicket.Description;
        
        EditButton_Click = new RelayCommand_(o =>
        {
            Console.WriteLine("Edit button clicked");
        });
        
        SaveButton_Click = new RelayCommand_(o =>
        {
            Console.WriteLine("Save button clicked");
            ticket.Status = "Pending";
            ticket.Description = OldDescription+$"\n \n {DateTime.Now}, Update by: {EditorId}, Department {EditorDepartmentId} \n \n "+_description;
            OldDescription = ticket.Description;
            ticket.DateTimeLastUpdated = DateTime.Now;
        });
        
        CancelButton_Click = new RelayCommand_(o =>
        {
            Console.WriteLine("Cancel button clicked");
        });
    }



}