using TicketeX_.Utilities;

namespace TicketeX_.ViewModels;

public class HomeViewModel: ObservableObject
{
    public string Username { get; set; }

    public HomeViewModel(string _username)
    {
        Username = "Witaj w TicketeX "+_username;
    }
}