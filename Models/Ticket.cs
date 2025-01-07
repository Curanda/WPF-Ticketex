using System.Data.SqlTypes;

namespace TicketeX_.Models;

public class Ticket
{
    public string TicketId { get; set; }
    public string Status { get; set; }
    public string Severity { get; set; }
    public string AuthorId { get; set; }
    public string Origin { get; set; }
    public string CurrentLocation { get; set; }
    public string PrevLocation { get; set; }
    public string ReporterId { get; set; }
    public DateTime DateTimeCreated { get; set; }
    public DateTime DateTimeLastUpdated { get; set; }
    public string Description { get; set; }
    public int NumOfUpdates { get; set; }
    public int NumOfUpVotes { get; set; }
    public int NumOfDownVotes { get; set; }
    public double VotesRatio { get; set; }
    public int Attachments { get; set; }
    public int SeverityNumber => SeverityToNumber();

    public static string DateTimeCsToSql(DateTime dateTime)
    {
        string sqlDateTime = new SqlDateTime(dateTime).ToString();
        return sqlDateTime;
    }

    public static DateTime SqlStringToDateTime(string sqlDateTime)
    {
        DateTime csDateTime = DateTime.Parse(sqlDateTime);
        return csDateTime;
    }

    public int SeverityToNumber()
    {
        int number = 0;
        if (Severity == "Low") number = 1;
        if (Severity == "Medium") number = 2;
        if (Severity == "High") number = 3;
        if (Severity == "Critical") number = 4;
        return number;
    }
    
}