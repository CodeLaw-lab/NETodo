using System.Windows;

using TodoApp.WPF.Core.Interfaces;

namespace TodoApp.WPF.Services;

public class ThemeService : IThemeService
{
   private readonly IConfigurationService _configurationService;

   public event EventHandler<string>? ThemeChanged;

   public ThemeService(IConfigurationService configurationService)
   {
      _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
   }

   public IEnumerable<string> GetAvailableThemes()
   {
      return new[] { "Light", "Dark", "Blue" };
   }

   public string GetCurrentTheme()
   {
      return _configurationService.AppSettings.Theme;
   }

   public async Task SetThemeAsync(string themeName)
   {
      if (!GetAvailableThemes().Contains(themeName))
         throw new ArgumentException($"Тема '{themeName}' не поддерживается");

      // Обновляем настройки
      _configurationService.AppSettings.Theme = themeName;
      await _configurationService.SaveAsync();

      // Применяем тему (упрощенная реализация)
      ApplyTheme(themeName);

      ThemeChanged?.Invoke(this, themeName);
   }

   private void ApplyTheme(string themeName)
   {
      Application.Current.Dispatcher.Invoke(() =>
      {
         // Удаляем существующие словари тем
         var dictionaries = Application.Current.Resources.MergedDictionaries;
         var dictionariesToRemove = dictionaries
            .Where(d => d.Source?.OriginalString?.Contains("Themes/") == true)
            .ToList();

         foreach (var dict in dictionariesToRemove)
         {
            dictionaries.Remove(dict);
         }

         // Добавляем выбранную тему
         var themeUri = new Uri($"Themes/{themeName}Theme.xaml", UriKind.Relative);
         dictionaries.Add(new ResourceDictionary { Source = themeUri });
      });
   }
}