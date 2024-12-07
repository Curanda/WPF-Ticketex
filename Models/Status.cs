using System.ComponentModel;
using TicketeX_.CustomAttributes;

namespace TicketeX_.Models;

public enum Status
{
    [EnumDescription("Open")]
    Open,
    [EnumDescription("Pending")]
    Pending,
    [EnumDescription("Closed")]
    Closed,
}