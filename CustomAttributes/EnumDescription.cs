using System.ComponentModel;

namespace TicketeX_.CustomAttributes;

public class EnumDescription: Attribute
{
    public string Description;

    public EnumDescription(string description)
    {
        Description = description;
    }

    public static string GetDescription(Enum value)
    {
        var type = value.GetType();
        var memInfo = type.GetMember(value.ToString());
        if (memInfo?.Length > 0)
        {
            var attrs = memInfo[0].GetCustomAttributes(typeof(EnumDescription), false);
            if (attrs?.Length > 0)
            {
                return ((EnumDescription)attrs[0]).Description;
            }
        }
        return value.ToString();
    }
}