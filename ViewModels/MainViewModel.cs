using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Messaging;
using Dapper;
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
    public RelayCommand_ ClosedTicketQueueViewCommand { get; set; }
    public static RelayCommand_ RefreshQueueCommand { get; set; }
    public static RelayCommand_ RefreshClosedTicketsCommand { get; set; }
    public RelayCommand_ LogoutCommand { get; set; }
    private LoggedUser LoggedUser { get; set; }
    private ObservableCollection<Ticket> TicketQueueTickets { get; set; } = [];
    private ObservableCollection<Ticket> ClosedTickets { get; set; } = [];
    private HomeViewModel HomeVm { get; set; }
    private TicketQueueViewModel TicketQueueVm { get; set; }
    private ClosedTicketQueueViewModel ClosedTicketQueueVm { get; set; }
    public CreateTicketViewModel CreateTicketVm { get; set; }
    private object _currentView;
    public string CurrentAccount { get; set; }
    
    public object CurrentView
    {
        get => _currentView;
        set {_currentView = value; OnPropertyChanged();}
    }

    public MainViewModel(LoggedUser loggedUser)
    {
        LoggedUser = loggedUser;
        CurrentAccount = loggedUser.Department;
        _currentView = HomeVm = new HomeViewModel(loggedUser.FirstName, loggedUser.Department);
        LoadTicketQueue();
        LoadClosedTicketQueue();
        
        HomeViewCommand = new RelayCommand_(o =>
        {
            CurrentView = HomeVm;
        });

        TicketQueueViewCommand = new RelayCommand_(o =>
        {
            CurrentView = new TicketQueueViewModel(TicketQueueTickets, LoggedUser);
        });

        ClosedTicketQueueViewCommand = new RelayCommand_(o =>
        {
            CurrentView = ClosedTicketQueueVm = new ClosedTicketQueueViewModel(ClosedTickets, LoggedUser);
        });

        CreateTicketViewCommand = new RelayCommand_(o =>
        {
            CurrentView = CreateTicketVm = new CreateTicketViewModel(LoggedUser);
        });

        RefreshQueueCommand = new RelayCommand_( o =>
        {
            ClearTicketQueue();
            LoadTicketQueue();
            
        });

        RefreshClosedTicketsCommand = new RelayCommand_(o =>
        {
            ClearTicketQueue();
            LoadClosedTicketQueue();
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

    }
    
    private async Task LoadTicketQueue()
    {

        string query = $"SELECT * FROM {CurrentAccount.ToLower()}_tickets";
        
        try
        {
            await using var connection = new MySqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            
            var tickets = await connection.QueryAsync<Ticket>(query);
            foreach (var ticket in tickets)
            {
                TicketQueueTickets.Add(ticket);
            }
            Console.WriteLine($"{LoggedUser.Department} ticket queue loaded");
            StrongReferenceMessenger.Default.Send(new StatusMessage("queue_refreshed"));
            Console.WriteLine($"Queue refreshed in viewmodel");
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task LoadClosedTicketQueue()
    {
        string query = $"SELECT * FROM closed_tickets";
        
        try
        {
            await using var connection = new MySqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            
            var tickets = await connection.QueryAsync<Ticket>(query);
            foreach (var ticket in tickets)
            {
                ClosedTickets.Add(ticket);
            }
            Console.WriteLine($"Closed ticket queue loaded");
            StrongReferenceMessenger.Default.Send(new StatusMessage("closed_queue_refreshed"));
            Console.WriteLine($"Closed tickets refreshed in viewmodel");
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    private void ClearTicketQueue()
    {
        TicketQueueTickets.Clear();
        Console.WriteLine("Clearing queue");
    }

}