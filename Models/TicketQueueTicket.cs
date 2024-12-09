namespace TicketeX_.Models;

public class TicketQueueTicket
{
    public string TicketId { get; set; }
    public string Status { get; set; }
    public string Severity { get; set; }
    public string AuthorId { get; set; }
    public string Origin { get; set; }
    public string PrevLocation { get; set; }
    public string ReporterId { get; set; }
    public DateTime DateTimeCreated { get; set; }
    public DateTime DateTimeLastUpdated { get; set; }
    public string Description { get; set; }
}