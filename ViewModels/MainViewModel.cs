using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;
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
    public static RelayCommand_ RefreshOpenTicketsCommand { get; set; }
    public static RelayCommand_ RefreshClosedTicketsCommand { get; set; }
    public RelayCommand_ LogoutCommand { get; set; }
    public LoggedUser LoggedUser { get; set; }
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
        
        HomeViewCommand = new RelayCommand_(o =>
        {
            CurrentView = HomeVm;
        });

        TicketQueueViewCommand = new RelayCommand_(async void (o) =>
        {
            TicketQueueTickets = await DatabaseEngine.LoadOpenQueue(CurrentAccount);
            CurrentView = TicketQueueVm = new TicketQueueViewModel(TicketQueueTickets, LoggedUser);
        });

        ClosedTicketQueueViewCommand = new RelayCommand_(async void (o) =>
        {
            ClosedTickets = await DatabaseEngine.LoadClosedQueue();
            CurrentView = ClosedTicketQueueVm = new ClosedTicketQueueViewModel(ClosedTickets, LoggedUser);
        });

        CreateTicketViewCommand = new RelayCommand_(o =>
        {
            CurrentView = CreateTicketVm = new CreateTicketViewModel(LoggedUser);
        });

        RefreshOpenTicketsCommand = new RelayCommand_( async void (o) =>
        {
            TicketQueueTickets.Clear();
            var newTickets = await DatabaseEngine.LoadOpenQueue(CurrentAccount);
            foreach (var ticket in newTickets)
            {
                TicketQueueTickets.Add(ticket);
            }

            if (CurrentView is not TicketQueueViewModel) return;
            TicketQueueVm = new TicketQueueViewModel(TicketQueueTickets, LoggedUser);
            CurrentView = TicketQueueVm;

        });

        RefreshClosedTicketsCommand = new RelayCommand_(async void (o) =>
        {
            ClosedTickets.Clear();
            var newTickets = await DatabaseEngine.LoadClosedQueue();
            foreach (var ticket in newTickets)
            {
                ClosedTickets.Add(ticket);
            }

            if (CurrentView is not ClosedTicketQueueViewModel) return;
            ClosedTicketQueueVm = new ClosedTicketQueueViewModel(ClosedTickets, LoggedUser);
            CurrentView = ClosedTicketQueueVm;
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
    
    private void ClearOpenTicketQueue()
    {
        TicketQueueTickets.Clear();
    }
    
    // private async Task LoadTicketQueue()
    // {
    //
    //     string query = $"SELECT * FROM {CurrentAccount.ToLower()}_tickets";
    //     
    //     try
    //     {
    //         await using var connection = new MySqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
    //         
    //         var tickets = await connection.QueryAsync<Ticket>(query);
    //         foreach (var ticket in tickets)
    //         {
    //             TicketQueueTickets.Add(ticket);
    //         }
    //         StrongReferenceMessenger.Default.Send(new StatusMessage("queue_refreshed"));
    //     }
    //     catch (MySqlException ex)
    //     {
    //         Console.WriteLine(ex.Message);
    //     }
    // }
    //
    // private async Task LoadClosedTicketQueue()
    // {
    //     string query = "SELECT * FROM closed_tickets";
    //     
    //     try
    //     {
    //         await using var connection = new MySqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
    //         
    //         var tickets = await connection.QueryAsync<Ticket>(query);
    //         foreach (var ticket in tickets)
    //         {
    //             ClosedTickets.Add(ticket);
    //         }
    //         StrongReferenceMessenger.Default.Send(new StatusMessage("closed_queue_refreshed"));
    //     }
    //     catch (MySqlException ex)
    //     {
    //         Console.WriteLine(ex.Message);
    //     }
    // }
}