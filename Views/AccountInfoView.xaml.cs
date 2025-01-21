using System.Windows;
using TicketeX_.Models;

namespace TicketeX_.Views;

public partial class AccountInfoView : Window
{
    public AccountInfoView(LoggedUser user)
    {
        InitializeComponent();
        Username.Text = user.UserId;
        Department.Text = user.Department;
        Firstname.Text = user.FirstName;
        Lastname.Text = user.LastName;
        Email.Text = user.Email;
        Phone.Text = user.Phone;
    }
}