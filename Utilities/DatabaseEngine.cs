using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Dapper;
using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using TicketeX_.Models;

namespace TicketeX_.Utilities;

public static class DatabaseEngine
{
    private static readonly string connectionString = ConfigurationManager.AppSettings["ConnectionString"];

    public static async Task<ObservableCollection<Ticket>> LoadOpenQueue(string department)
    {
        var tickets = new ObservableCollection<Ticket>();
        var query = $"SELECT * FROM {department.ToLower()}_tickets";
        
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
        var query = "SELECT * FROM closed_tickets";
        
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

    private static async Task<bool> InsertTicket(Ticket ticket)
    {
        
        try
        {
            await using var connection = new MySqlConnection(connectionString);
            var department = ticket.Status == "Closed" ? "closed" : ticket.CurrentLocation.ToLower();
            var query = $@"INSERT INTO {department}_tickets 
                          (TicketId, Status, Severity, AuthorId, Origin, CurrentLocation, 
                           PrevLocation, ReporterId, DateTimeCreated, DateTimeLastUpdated, Description, NumOfUpdates, NumOfUpVotes, 
                           NumOfDownVotes, VotesRatio, Attachments) 
                          VALUES (@TicketId, @Status, @Severity, @AuthorId, @Origin, 
                                 @CurrentLocation, @PrevLocation, @ReporterId, @DateTimeCreated, @DateTimeLastUpdated, @Description, 
                                 @NumOfUpdates, @NumOfUpVotes, @NumOfDownVotes, @VotesRatio, 
                                 @Attachments)";
            
            var rowsAffected = await connection.ExecuteAsync(query, ticket);
            return rowsAffected > 0;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine("Failed to insert new ticket into database. "+ex.Message); 
            return false;
        }
    }
    
    private static async Task<string> GenerateNewTicketNumber(string targetDepartment)
    {
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        var query = @"SELECT TotalTickets FROM ticket_counter WHERE Department = @targetDepartment FOR UPDATE";
        
        try
        {
            var newTicketNumber = connection.Query<int>(query, new { targetDepartment }).FirstOrDefault()+1;
            var newTicketId = $"{targetDepartment}-{newTicketNumber}";
            const string updateQuery = "UPDATE ticket_counter SET TotalTickets = @newTicketNumber WHERE Department = @targetDepartment";
            var isSuccess = await connection.ExecuteAsync(updateQuery, new { newTicketNumber, targetDepartment });
            return newTicketId;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
            return "error";
        }
    }

    public static async Task<bool> CreateTicket(Ticket ticket)
    {
        var newTicketId = await GenerateNewTicketNumber(ticket.Origin.ToLower());
        if (newTicketId == "error") return false;
        ticket.TicketId = newTicketId;
        var isSuccess = await InsertTicket(ticket);
        return isSuccess;
    }
    public static async Task<bool> UpdateTicket(Ticket ticket)
    {
        try
        {
            await using var connection = new MySqlConnection(connectionString);
            var query = $@"UPDATE {ticket.CurrentLocation.ToLower()}_tickets 
                          SET Status = @Status, Description = @Description, 
                              NumOfUpdates = @NumOfUpdates, DateTimeLastUpdated = @DateTimeLastUpdated 
                          WHERE TicketId = @TicketId";
            
            var rowsAffected = await connection.ExecuteAsync(query, ticket);
            return rowsAffected > 0;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
    

    private static async Task<bool> DeleteTicket(Ticket ticket, string destination)
    {
        try
        {
            await using var connection = new MySqlConnection(connectionString);
            var tableName = destination.ToLower() == "closed" ? "closed_tickets" : $"{destination.ToLower()}_tickets";
            var query = $"DELETE FROM {tableName} WHERE TicketId = @TicketId";
            Console.WriteLine($"Executing delete query: {query} with TicketId: {ticket.TicketId}");
            var rowsAffected = await connection.ExecuteAsync(query, new { ticket.TicketId });
            return rowsAffected > 0;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public static async Task<bool> TransferTicket(Ticket ticket)
    {
        Console.WriteLine("Transfer called");
        var isInsertSuccess = await InsertTicket(ticket);
        if (!isInsertSuccess) return false;
        var isDeleteSuccess = await DeleteTicket(ticket, ticket.PrevLocation);
        if (!isDeleteSuccess) 
            MessageBox.Show($"Partial error transferring ticket {ticket.TicketId}. Ticket delegated correctly but deletion from old queue {ticket.PrevLocation} failed. Please contact Dbsupport");
        return true;
    }
    
    public static async Task<bool> CloseTicket(Ticket ticket)
    {
        Console.WriteLine("Close called");
        var isInsertSuccess = await InsertTicket(ticket);
        if (!isInsertSuccess) return false;
        var isDeleteSuccess = await DeleteTicket(ticket, ticket.CurrentLocation);
        if (!isDeleteSuccess) 
            MessageBox.Show($"Partial error closing ticket {ticket.TicketId}. Ticket made it to the Archive but deletion from open queue {ticket.CurrentLocation} failed. Please contact Dbsupport");
        return true;
    }
    
    public static async Task<bool> ReopenTicket(Ticket ticket)
    {
        Console.WriteLine("Reopen called");
        var isInsertSuccess = await InsertTicket(ticket);
        if (!isInsertSuccess) Console.WriteLine("Reopening: Insert failure");
        if (!isInsertSuccess) return false;
        var isDeleteSuccess = await DeleteTicket(ticket, "closed");
        if (!isDeleteSuccess) 
            MessageBox.Show($"Partial error reopening ticket {ticket.TicketId}. Ticket made it to an open queue {ticket.CurrentLocation} but deletion from Archive has failed. Please contact Dbsupport");
        return true;
    }
    
    public static async Task<LoggedUser> LoginUser(string username, PasswordBox passwordBox)
    {
        var password = passwordBox.Password;
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        var query = "SELECT UserId, FirstName, LastName, Email, Phone, Department FROM all_users WHERE UserId = @UserId AND Password = @Password";
        
        try
        {
            var res = connection.Query<LoggedUser>(query, new 
            { 
                UserId = username, 
                Password = password
            }).FirstOrDefault();
            return res;
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
    
}