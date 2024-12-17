using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
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
    public static RelayCommand_ RefreshQueueCommand { get; set; }
    public RelayCommand_ LogoutCommand { get; set; }
    private LoggedUser _loggedUser { get; set; }
    private ObservableCollection<Ticket> TicketQueueTickets { get; set; } = new();
    private HomeViewModel HomeVm { get; set; }
    private TicketQueueViewModel TicketQueueVm { get; set; }
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
        // Console.WriteLine(loggedUser.Department);
        _loggedUser = loggedUser;
        CurrentAccount = loggedUser.Department;
        _currentView = HomeVm = new HomeViewModel(loggedUser.FirstName, loggedUser.Department);
        TicketQueueVm = new TicketQueueViewModel(TicketQueueTickets, _loggedUser);
        LoadTicketQueue(CurrentAccount);
        
        HomeViewCommand = new RelayCommand_(o =>
        {
            CurrentView = HomeVm;
        });

        TicketQueueViewCommand = new RelayCommand_(o =>
        {
            CurrentView = TicketQueueVm;
        });

        CreateTicketViewCommand = new RelayCommand_(o =>
        {
            CurrentView = CreateTicketVm = new CreateTicketViewModel(_loggedUser);
        });

        RefreshQueueCommand = new RelayCommand_( o =>
        {
            ClearTicketQueue();
            LoadTicketQueue(CurrentAccount);
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
    
    private async Task LoadTicketQueue(string CurrentAccount)
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
            Console.WriteLine($"{_loggedUser.Department} ticket queue loaded");
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