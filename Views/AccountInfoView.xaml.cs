using System.Windows;
using TicketeX_.Models;

namespace TicketeX_.Views;

public partial class AccountInfoView : Window
{
    public AccountInfoView(LoggedUser user)
    {
        InitializeComponent();
        Username.Content = user.UserId;
        Department.Content = user.Department;
        Firstname.Content = user.FirstName;
        Lastname.Content = user.LastName;
        Email.Content = user.Email;
        Phone.Content = user.Phone;
    }
}