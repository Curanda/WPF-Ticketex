namespace TicketeX_.Models;

public class TicketQueueTicket
{
    public int TicketId { get; set; }
    public string Status { get; set; }
    public string Severity { get; set; }
    public string AuthorId { get; set; }
    public string OriginId { get; set; }
    public string Description { get; set; }
    public int? UpVotes { get; set; }
    public int? DownVotes { get; set; }
    public double? VotesRatio { get; set; }
}