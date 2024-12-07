using Google.Protobuf.Reflection;

namespace TicketeX_.Models;

public class TicketCounter
{
    public Location Department { get; set; }
    public int TotalTickets { get; set; }
}