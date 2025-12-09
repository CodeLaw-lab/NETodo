using System.Globalization;
using System.Windows.Data;
using TodoApp.WPF.Core.Enums;

namespace TodoApp.WPF.Converters;

public class PriorityToStringConverter : IValueConverter
{
   public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
   {
      if (value is Priority priority)
      {
         return priority switch
         {
            Priority.Low => "Низкий",
            Priority.Medium => "Средний",
            Priority.High => "Высокий",
            Priority.Critical => "Критический",
            _ => "Неизвестно"
         };
      }
      return "Неизвестно";
   }

   public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
   {
      throw new NotImplementedException();
   }
}