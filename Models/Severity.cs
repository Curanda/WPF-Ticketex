using System.ComponentModel;
using TicketeX_.CustomAttributes;

namespace TicketeX_.Models;

public enum Severity 
{
    [EnumDescription("High")]
    Low,
    [EnumDescription("Medium")]
    Medium,
    [EnumDescription("Low")]
    High,
    [EnumDescription("Medium")]
    Critical
}