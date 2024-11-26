using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TicketeX_.Utilities;

public class ObservableObject: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));   
    }
}