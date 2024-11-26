namespace TicketeX_.Models;

public struct ToDbTicket
{
    public string Status { get; set; }
    public string Severity { get; set; }
    public string AuthorId { get; set; }
    public string OriginId { get; set; }
    public string CurrentLocId { get; set; }
    public string LastLocId { get; set; }
    public string ReporterId { get; set; }
    public string Description { get; set; }
}