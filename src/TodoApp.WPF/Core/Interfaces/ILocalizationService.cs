using System.Globalization;

namespace TodoApp.WPF.Core.Interfaces;

public interface ILocalizationService
{
   IEnumerable<CultureInfo> GetAvailableCultures();
   CultureInfo GetCurrentCulture();
   Task SetCultureAsync(CultureInfo culture);
   event EventHandler<CultureInfo> CultureChanged;
}