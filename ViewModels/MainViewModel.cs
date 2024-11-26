using System.Collections.ObjectModel;
using Dapper;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using TicketeX_.Models;
using TicketeX_.Views;

namespace TicketeX_.ViewModels;
using TicketeX_.Utilities;


public class MainViewModel: ObservableObject
{
    
    public RelayCommand HomeViewCommand { get; set; }
    public RelayCommand TicketQueueViewCommand { get; set; }
    public RelayCommand CreateTicketViewCommand { get; set; }
    
    public RelayCommand SwitchAccountCommand { get; set; }
    private ObservableCollection<TicketQueueTicket> TicketQueueTickets { get; set; } = new();
    private HomeViewModel HomeVm { get; set; }
    public TicketQueueViewModel TicketQueueVm { get; set; }
    public LoginViewModel LoginVm { get; set; }
    public CreateTicketViewModel CreateTicketVm { get; set; }
    private object _currentView;
    
    private LoggedUser _loggedUser;
    public string HelpdeskAccount { get; set; } = "Helpdesk";
    public string WsupAccount { get; set; } = "Windows Support";
    public string NetsupAccount { get; set; } = "Network Support";
    public string HrAccount { get; set; } = "Hr";
    public string LagerAccount { get; set; } = "Lager";
    public string DbsupportAccount { get; set; } = "DB support";
    public string TechnicianAccount { get; set; } = "Technician";
    public string JanitorAccount { get; set; } = "Janitor";
    
    public string CurrentAccount { get; set; }
    

    public object CurrentView
    {
        get => _currentView;
        set {_currentView = value; OnPropertyChanged();}
    }

    public MainViewModel(LoggedUser loggedUser)
    {
        _loggedUser = loggedUser;
        Console.WriteLine(_loggedUser.Department);
        CurrentAccount = loggedUser.Department;
        _currentView = HomeVm = new HomeViewModel(loggedUser.FirstName);
        TicketQueueVm = new TicketQueueViewModel(TicketQueueTickets);
        
        
        HomeViewCommand = new RelayCommand(o =>
        {
            CurrentView = HomeVm;
            // LoginVm = new LoginViewModel();
            // LoginView loginView = new LoginView();
            // loginView.DataContext = LoginVm;
            // loginView.Show();
        });

        TicketQueueViewCommand = new RelayCommand(o =>
        {
            CurrentView = TicketQueueVm;
            LoadHelpdeskTickets(CurrentAccount);
        });

        CreateTicketViewCommand = new RelayCommand(o =>
        {
            CurrentView = CreateTicketVm = new CreateTicketViewModel();
        });

        SwitchAccountCommand = new RelayCommand( account =>
        {
            string newCurrentAccount = account.ToString();
            ClearTicketQueue();
            LoadHelpdeskTickets(newCurrentAccount);
        });
    }



    public async void LoadHelpdeskTickets(string account)
    {

        string query;
        // 'Helpdesk', 'Windowssupport', 'Networksupport', 'Hr', 'Lager', 'Dbsupport', 'Technician', 'Janitor'
        switch (account)
        {
            case "Windows Support":
                query = "SELECT * FROM wsup_ticket_queue";
                break;
            case "Network Support":
                query = "SELECT * FROM networksup_ticket_queue";
                break;
            case "Hr":
                query = "SELECT * FROM hr_ticket_queue";
                break;
            case "Lager":
                query = "SELECT * FROM lager_ticket_queue";
                break;
            case "DB support":
                query = "SELECT * FROM helpdesk_ticket_queue";
                break;
            case "Technician":
                query = "SELECT * FROM helpdesk_ticket_queue";
                break;
            case "Janitor":
                query = "SELECT * FROM helpdesk_ticket_queue";
                break;
            default:
                query = "SELECT * FROM helpdesk_ticket_queue";
                break;
        }
        
        try
        {

            using var connection = new MySqlConnection("Server=localhost;Database=ticketex_;User=root;Password=root;");
            
            var tickets = await connection.QueryAsync<TicketQueueTicket>(query);
            foreach (var ticket in tickets)
            {
                TicketQueueTickets.Add(ticket);
            }
            
        }
        catch (SqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    private void ClearTicketQueue()
    {
        TicketQueueTickets.Clear();
    }

}