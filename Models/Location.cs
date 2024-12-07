using System.Runtime.InteropServices.ComTypes;
using TicketeX_.CustomAttributes;

namespace TicketeX_.Models;

public enum Location
{
    [EnumDescription("Helpdesk")]
    Helpdesk,
    [EnumDescription("Windowssupport")]
    Windowssupport,
    [EnumDescription("Networksupport")]
    Networksupport,
    [EnumDescription("Hr")]
    Hr,
    [EnumDescription("Lager")]
    Lager,
    [EnumDescription("Dbsupport")]
    Dbsupport,
    [EnumDescription("Technician")]
    Technician,
    [EnumDescription("Janitor")]
    Janitor
}