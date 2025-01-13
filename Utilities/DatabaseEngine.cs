using System.Collections.ObjectModel;
using System.Configuration;
using CommunityToolkit.Mvvm.Messaging;
using Dapper;
using MySql.Data.MySqlClient;
using TicketeX_.Models;

namespace TicketeX_.Utilities;

public static class DatabaseEngine
{
    private static readonly string connectionString = ConfigurationManager.AppSettings["ConnectionString"];

    public static async Task<ObservableCollection<Ticket>> LoadOpenQueue(string department)
    {
        var tickets = new ObservableCollection<Ticket>();
        string query = $"SELECT * FROM {department.ToLower()}_tickets";
        
        try
        {
            await using var connection = new MySqlConnection(connectionString);
            var results = await connection.QueryAsync<Ticket>(query);
            foreach (var ticket in results)
            {
                tickets.Add(ticket);
            }
            // StrongReferenceMessenger.Default.Send(new StatusMessage("queue_refreshed"));
            return tickets;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
            return tickets;
        }
    }

    public static async Task<ObservableCollection<Ticket>> LoadClosedQueue()
    {
        var tickets = new ObservableCollection<Ticket>();
        string query = "SELECT * FROM closed_tickets";
        
        try
        {
            await using var connection = new MySqlConnection(connectionString);
            var results = await connection.QueryAsync<Ticket>(query);
            foreach (var ticket in results)
            {
                tickets.Add(ticket);
            }
            Console.WriteLine("Closed Tickets from dbengine");
            return tickets;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
            return tickets;
        }
    }

    public static async Task<bool> CreateTicket(Ticket ticket, string destinationDepartment)
    {
        try
        {
            await using var connection = new MySqlConnection(connectionString);
            var query = $@"INSERT INTO {destinationDepartment.ToLower()}_tickets 
                          (TicketId, Status, Severity, AuthorId, Origin, CurrentLocation, 
                           PrevLocation, ReporterId, Description, NumOfUpdates, NumOfUpVotes, 
                           NumOfDownVotes, VotesRatio, Attachments) 
                          VALUES (@TicketId, @Status, @Severity, @AuthorId, @Origin, 
                                 @CurrentLocation, @PrevLocation, @ReporterId, @Description, 
                                 @NumOfUpdates, @NumOfUpVotes, @NumOfDownVotes, @VotesRatio, 
                                 @Attachments)";
            
            var rowsAffected = await connection.ExecuteAsync(query, ticket);
            return rowsAffected > 0;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
    //
    // public static async Task<bool> UpdateTicket(Ticket ticket)
    // {
    //     try
    //     {
    //         await using var connection = new MySqlConnection(connectionString);
    //         var query = $@"UPDATE {ticket.CurrentLocation.ToLower()}_tickets 
    //                       SET Status = @Status, Description = @Description, 
    //                           NumOfUpdates = @NumOfUpdates, DateTimeLastUpdated = @DateTimeLastUpdated 
    //                       WHERE TicketId = @TicketId";
    //         
    //         int rowsAffected = await connection.ExecuteAsync(query, ticket);
    //         return rowsAffected > 0;
    //     }
    //     catch (MySqlException ex)
    //     {
    //         Console.WriteLine(ex.Message);
    //         return false;
    //     }
    // }
    //
    // public static async Task<bool> DeleteTicket(Ticket ticket, string department)
    // {
    //     try
    //     {
    //         await using var connection = new MySqlConnection(connectionString);
    //         var query = $"DELETE FROM {department.ToLower()}_tickets WHERE TicketId = @TicketId";
    //         int rowsAffected = await connection.ExecuteAsync(query, new { ticket.TicketId });
    //         return rowsAffected > 0;
    //     }
    //     catch (MySqlException ex)
    //     {
    //         Console.WriteLine(ex.Message);
    //         return false;
    //     }
    // }
    //
    // public static async Task<bool> TransferTicket(Ticket ticket, string targetDepartment)
    // {
    //     try
    //     {
    //         await using var connection = new MySqlConnection(connectionString);
    //         
    //         // Delete from current department
    //         bool deleteSuccess = await DeleteTicket(ticket, ticket.CurrentLocation);
    //         if (!deleteSuccess) return false;
    //
    //         // Update ticket location
    //         ticket.PrevLocation = ticket.CurrentLocation;
    //         ticket.CurrentLocation = targetDepartment;
    //
    //         // Insert into new department
    //         return await CreateTicket(ticket, targetDepartment);
    //     }
    //     catch (MySqlException ex)
    //     {
    //         Console.WriteLine(ex.Message);
    //         return false;
    //     }
    // }
    //
    // public static async Task<bool> CloseTicket(Ticket ticket)
    // {
    //     try
    //     {
    //         await using var connection = new MySqlConnection(connectionString);
    //         
    //         // Delete from current department
    //         bool deleteSuccess = await DeleteTicket(ticket, ticket.CurrentLocation);
    //         if (!deleteSuccess) return false;
    //
    //         // Insert into closed tickets
    //         var query = @"INSERT INTO closed_tickets 
    //                      (TicketId, Status, Severity, AuthorId, Origin, CurrentLocation, 
    //                       PrevLocation, ReporterId, Description, NumOfUpdates, NumOfUpVotes, 
    //                       NumOfDownVotes, VotesRatio, Attachments) 
    //                      VALUES (@TicketId, @Status, @Severity, @AuthorId, @Origin, 
    //                             @CurrentLocation, @PrevLocation, @ReporterId, @Description, 
    //                             @NumOfUpdates, @NumOfUpVotes, @NumOfDownVotes, @VotesRatio, 
    //                             @Attachments)";
    //         
    //         int rowsAffected = await connection.ExecuteAsync(query, ticket);
    //         return rowsAffected > 0;
    //     }
    //     catch (MySqlException ex)
    //     {
    //         Console.WriteLine(ex.Message);
    //         return false;
    //     }
    // }
    //
    // public static async Task<(bool isSuccess, string department)> ValidateLogin(string username, string password)
    // {
    //     try
    //     {
    //         await using var connection = new MySqlConnection(connectionString);
    //         var query = "SELECT Department FROM all_users WHERE UserId = @Username AND Password = @Password";
    //         var department = await connection.QueryFirstOrDefaultAsync<string>(query, new { Username = username, Password = password });
    //         
    //         return (department != null, department ?? string.Empty);
    //     }
    //     catch (MySqlException ex)
    //     {
    //         Console.WriteLine(ex.Message);
    //         return (false, string.Empty);
    //     }
    // }
}

// - ladowanie ticketow do kolejki w mainviewmodel (department) return TicketQueue albo obiekt db;  
// - wysylanie nowych ticketow w createNewTicketViewModel (ticket) return bool isSuccess;
// - update ticketow w TicketqueueViewModel (ticket) return bool isSuccess;
// - kasowanie ticketow w TicketqueueViewModel (ticket) return bool isSuccess
// - wysylanie nowych ticketow w TicketqueueViewModel (ticket)  return bool isSuccess
// - wysylanie nowych ticketow w ClosedTicketQueueViewModel (ticket)  return bool isSuccess
// - kasowanie ticketow w ClosedTicketQueueViewModel (ticket) return bool isSuccess


// - logowanie w LoginViewModel(username, password) return bool isSuccess, string department