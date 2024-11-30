using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Dapper;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using TicketeX_.Models;
using TicketeX_.Utilities;
using TicketeX_.Views;

namespace TicketeX_.ViewModels;


public class MainViewModel: ObservableObject
{
    public RelayCommand_ HomeViewCommand { get; set; }
    public RelayCommand_ TicketQueueViewCommand { get; set; }
    public RelayCommand_ CreateTicketViewCommand { get; set; }
    public RelayCommand_ SwitchAccountCommand { get; set; }
    public RelayCommand_ LogoutCommand { get; set; }
    public ICommand CloseWindowCommand { get; private set; }
    private ObservableCollection<TicketQueueTicket> TicketQueueTickets { get; set; } = new();
    private HomeViewModel HomeVm { get; set; }
    private TicketQueueViewModel TicketQueueVm { get; set; }
    public CreateTicketViewModel CreateTicketVm { get; set; }
    private object _currentView;

    public string HelpdeskAccount { get; set; } = "Helpdesk";
    public string WsupAccount { get; set; } = "Windows Support";
    public string NetsupAccount { get; set; } = "Network Support";
    public string HrAccount { get; set; } = "Hr";
    public string LagerAccount { get; set; } = "Lager";
    public string DbsupportAccount { get; set; } = "DB support";
    public string TechnicianAccount { get; set; } = "Technician";
    public string JanitorAccount { get; set; } = "Janitor";
    private string CurrentAccount { get; set; }
    
    public object CurrentView
    {
        get => _currentView;
        set {_currentView = value; OnPropertyChanged();}
    }

    public MainViewModel(LoggedUser loggedUser)
    {
        Console.WriteLine(loggedUser.Department);
        CurrentAccount = loggedUser.Department;
        _currentView = HomeVm = new HomeViewModel(loggedUser.FirstName);
        TicketQueueVm = new TicketQueueViewModel(TicketQueueTickets);
        
        
        HomeViewCommand = new RelayCommand_(o =>
        {
            CurrentView = HomeVm;
        });

        TicketQueueViewCommand = new RelayCommand_(o =>
        {
            CurrentView = TicketQueueVm;
            LoadTicketQueue(CurrentAccount);
        });

        CreateTicketViewCommand = new RelayCommand_(o =>
        {
            CurrentView = CreateTicketVm = new CreateTicketViewModel();
        });

        SwitchAccountCommand = new RelayCommand_( account =>
        {
            string newCurrentAccount = account.ToString();
            ClearTicketQueue();
            LoadTicketQueue(newCurrentAccount);
        });

        LogoutCommand = new RelayCommand_(o =>
        {
            LoginViewModel loginVm = new LoginViewModel();
            LoginView loginView = new LoginView
            {
                DataContext = loginVm
            };
            loginView.Show(); 
            foreach (Window window in Application.Current.Windows)
            {
                if (window is not LoginView)
                {
                    window.Close();
                }
            }
        });

        CloseWindowCommand = new RelayCommand_(o =>
        {
            LoginViewModel loginVm = new LoginViewModel();
            LoginView loginView = new LoginView
            {
                DataContext = loginVm
            };
            loginView.Show(); 
            foreach (Window window in Application.Current.Windows)
            {
                if (window is not LoginView)
                {
                    window.Close();
                }
            }
        });

    }


    
    

    private async Task LoadTicketQueue(string account)
    {

        string query;
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

            await using var connection = new MySqlConnection("Server=localhost;Database=ticketex_;User=root;Password=root;");
            
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