using System.Windows;
using System.Windows.Controls;
using Dapper;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Mysqlx;
using TicketeX_.Models;
using TicketeX_.Utilities;

namespace TicketeX_.ViewModels;

public class CreateTicketViewModel: ObservableObject
{
    public RelayCommand_ CreateTicketCommand { get; }
    private string _selectedSeverity;
    private string _selectedDestination;
    private string _author;
    private string _reportedBy;
    private string _description;

    public ComboBoxItem SelectedSeverity
    {
        set
        {
            _selectedSeverity = value.Content.ToString();
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
    public CreateTicketViewModel()
    {
      CreateTicketCommand = new RelayCommand_(CreateTicket);
    }

    private void CreateTicket(object parameter)
    {
        var helpdesk_ticket = new ToDbTicket
        {
            Status = "Open",
            Severity = _selectedSeverity,
            AuthorId = Author,
            OriginId = "Helpdesk",
            CurrentLocId = _selectedDestination,
            LastLocId = "Helpdesk",
            ReporterId = ReportedBy,
            Description = Description,
        };
        SendTicketToDB(helpdesk_ticket);
        MessageBox.Show("Ticket created successfully!");
    }


    private void SendTicketToDB(ToDbTicket helpdesk_ticket)
    {
        try
        {

            using var connection = new MySqlConnection("Server=localhost;Database=ticketex_;User=root;Password=root;");
            var sql = "INSERT INTO ticketex_.all_tickets (Status, Severity, AuthorId, OriginId, CurrentLocId, LastLocId, ReporterId, Description) VALUES (@Status, @Severity, @AuthorId, @OriginId, @CurrentLocId, @LastLocId, @ReporterId, @Description)";
            connection.Execute(sql, helpdesk_ticket);
        }
        catch (SqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
}