using TicketeX_.Utilities;

namespace TicketeX_.ViewModels;

public class HomeViewModel: ObservableObject
{
    public string Username { get; set; }
    public string Department { get; set; }

    public HomeViewModel(string _username, string Department)
    {
        Username = "Welcome to TicketeX, "+_username;
        this.Department = "Department: "+Department;
    }
}