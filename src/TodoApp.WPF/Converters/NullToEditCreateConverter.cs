using System.Globalization;
using System.Windows.Data;
using TodoApp.WPF.Core.Entities;

namespace TodoApp.WPF.Converters;

public class NullToEditCreateConverter : IValueConverter
{
   public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
   {
      if (value is Category)
         return "Редактирование категории";
      else
         return "Создание категории";
   }

   public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
   {
      throw new NotImplementedException();
   }
}