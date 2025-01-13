using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace TicketeX_.Utilities;

public class ValidationErrorsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ReadOnlyObservableCollection<ValidationError> errors && parameter is string propertyName)
        {
            var error = errors.FirstOrDefault(e => e.ErrorContent.ToString().Contains(propertyName));
            return error?.ErrorContent;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}