using TicketeX_.CustomAttributes;

namespace TicketeX_.Models;

public enum Severity
{
    [EnumDescription("Low")]
    Low,
    [EnumDescription("Medium")]
    Medium,
    [EnumDescription("High")]
    High,
    [EnumDescription("Critical")]
    Critical
}