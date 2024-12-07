namespace TicketeX_.Models;

public class Ticket_(
    string ticketId,
    Location origin,
    Status status,
    Severity severity,
    string authorId,
    Location currentLocation,
    Location prevLocation,
    string reporterId,
    DateTime dateTimeCreated,
    DateTime dateTimeLastUpdated,
    string description,
    int numOfUpdates,
    int numOfUpVotes,
    int numOfDownVotes,
    double votesRatio,
    int attachments)
{
    public string TicketId { get; set; } = ticketId;
    public Status Status { get; set; } = status;
    public Severity Severity { get; set; } = severity;
    public string AuthorId { get; set; } = authorId;
    public Location Origin { get; set; } = origin;
    public Location CurrentLocation { get; set; } = currentLocation;
    public Location PrevLocation { get; set; } = prevLocation;
    public string ReporterId { get; set; } = reporterId;
    public DateTime DateTimeCreated { get; set; } = dateTimeCreated;
    public DateTime DateTimeLastUpdated { get; set; } = dateTimeLastUpdated;
    public string Description { get; set; } = description;
    public int NumOfUpdates { get; set; } = numOfUpdates;
    public int NumOfUpVotes { get; set; } = numOfUpVotes;
    public int NumOfDownVotes { get; set; } = numOfDownVotes;
    public double VotesRatio { get; set; } = votesRatio;
    public int Attachments { get; set; } = attachments;
}