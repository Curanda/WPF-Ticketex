using Google.Protobuf.Reflection;
using TicketeX_.Models;

namespace TicketeX_.CustomAttributes;

public class CastLocationToEnum: Attribute
{
    public string targetString;
    public static Location location { get; set; }

    public CastLocationToEnum(string targetString)
    {
        this.targetString = targetString;
    } 
    
    public static void castLocationToEnum(string targetString)
    {
        foreach (Location item in Enum.GetValues(typeof(Location)))
        {
            if (targetString.Equals(item.ToString()))
            {
                location = item;
                break;
            }
        }
    }
}