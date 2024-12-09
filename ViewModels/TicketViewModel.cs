using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using TicketeX_.Models;
using TicketeX_.Utilities;

namespace TicketeX_.ViewModels;

public class TicketViewModel
{
    private Ticket ticket;
    
    private readonly string _connectionString = ConfigurationManager.AppSettings["ConnectionString"];
    
    private string _selectedSeverity { get; set; }
    public RelayCommand_ EditButton_Click { get; }
    public RelayCommand_ SaveButton_Click { get; }
    public RelayCommand_ CancelButton_Click { get; }
    private string _selectedDestination { get; set; }
    private string _description { get; set; }


    public ComboBoxItem SelectedDestination
    {
        set
        {
            _selectedDestination = value.Content.ToString();
            // OnPropertyChanged();
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
        _description = ticket.Description;
        EditButton_Click = new RelayCommand_(o =>
        {
            Console.WriteLine("Edit button clicked");
        });
        SaveButton_Click = new RelayCommand_(o =>
        {
            Console.WriteLine("Save button clicked");
            ticket.Status = "Pending";
            ticket.Description = _description;
            ticket.DateTimeLastUpdated = DateTime.Now;
        });
        CancelButton_Click = new RelayCommand_(o =>
        {
            Console.WriteLine("Cancel button clicked");
        });
    }



}