using Google.Protobuf.Reflection;
using TicketeX_.Models;

namespace TicketeX_.CustomAttributes;

public class CastLocationToEnum: Attribute
{
    public string targetString;
    public static Location_ location { get; set; }

    public CastLocationToEnum(string targetString)
    {
        this.targetString = targetString;
    } 
    
    public static void castLocationToEnum(string targetString)
    {
        foreach (Location_ item in Enum.GetValues(typeof(Location_)))
        {
            if (targetString.Equals(item.ToString()))
            {
                location = item;
                break;
            }
        }
    }
}