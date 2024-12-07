namespace TicketeX_.Utilities;

public class DateTimeConverterForSQL
{
    public static DateTime CsToSql(DateTime dateTime)
    {
        dateTime = dateTime.ToUniversalTime();
    }
}