namespace TicketeX_.Models;

public class TicketQueueTicket_
{
    public string TicketId { get; set; }
    public Status Status { get; set; }
    public Severity Severity { get; set; }
    public string AuthorId { get; set; }
    public Location Origin { get; set; }
    public Location PrevLocation { get; set; }
    public string ReporterId { get; set; }
    public DateTime DateTimeCreated { get; set; }
    public DateTime DateTimeLastUpdated { get; set; }
    public string Description { get; set; }
}