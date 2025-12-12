using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TodoApp.WPF.Converters;

public class IntToVisibilityConverter : IValueConverter
{
   public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
   {
      if (value is int intValue && parameter is string param)
      {
         var minLength = int.Parse(param);
         return intValue < minLength ? Visibility.Visible : Visibility.Collapsed;
      }
      return Visibility.Collapsed;
   }

   public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
   {
      throw new NotImplementedException();
   }
}