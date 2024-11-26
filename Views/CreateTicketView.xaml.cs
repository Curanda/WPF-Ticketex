using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Regex = System.Text.RegularExpressions.Regex;

namespace TicketeX_.Views;

public partial class CreateTicketView : UserControl
{
    public CreateTicketView()
    {
        InitializeComponent();
    }

    private void ValidationAtoZ_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex ( "[^a-zA-Z]+" );
        if ( regex.IsMatch ( AuthorTextBox.Text ) )
        {
            MessageBox.Show("Invalid author Id. Only letters are allowed.");
        } 
        if ( regex.IsMatch ( ReportedByTextBox.Text ) )
        {
            MessageBox.Show("Invalid reported by. Only letters are allowed.");
        } 
    }
    
}