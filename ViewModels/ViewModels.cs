using System.Collections.ObjectModel;
using TicketeX_.Models;

namespace TicketeX_.ViewModels;

public class ViewModels
{
    public static LoggedUser LoggedUser;
    
    public static object loginViewModel { get; } = new LoginViewModel();
    public static object mainViewModel { get; } = new MainViewModel();
}