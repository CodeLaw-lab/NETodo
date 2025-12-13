using System.Globalization;
using System.Windows;
using TodoApp.WPF.Core.Interfaces;

namespace TodoApp.WPF.Services;

public class LocalizationService : ILocalizationService
{
   private readonly IConfigurationService _configurationService;

   public event EventHandler<CultureInfo>? CultureChanged;

   public LocalizationService(IConfigurationService configurationService)
   {
      _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
   }

   public IEnumerable<CultureInfo> GetAvailableCultures()
   {
      return new[]
      {
         new CultureInfo("ru-RU"),
         new CultureInfo("en-US")
      };
   }

   public CultureInfo GetCurrentCulture()
   {
      var cultureName = _configurationService.AppSettings.Culture;
      try
      {
         return new CultureInfo(cultureName);
      }
      catch
      {
         return CultureInfo.CurrentUICulture;
      }
   }

   public async Task SetCultureAsync(CultureInfo culture)
   {
      if (!GetAvailableCultures().Any(c => c.Name == culture.Name))
         throw new ArgumentException($"Культура '{culture.Name}' не поддерживается");

      // Сохраняем настройки
      _configurationService.AppSettings.Culture = culture.Name;
      await _configurationService.SaveAsync();

      // Устанавливаем культуру для текущего потока
      Thread.CurrentThread.CurrentCulture = culture;
      Thread.CurrentThread.CurrentUICulture = culture;

      // Обновляем ресурсы WPF
      UpdateResourceDictionary(culture);

      CultureChanged?.Invoke(this, culture);
   }

   private void UpdateResourceDictionary(CultureInfo culture)
   {
      Application.Current.Dispatcher.Invoke(() =>
      {
         var dictionaries = Application.Current.Resources.MergedDictionaries;

         // Удаляем существующие словари локализации
         var dictionariesToRemove = dictionaries
            .Where(d => d.Source?.OriginalString?.Contains("Resources/Strings") == true)
            .ToList();

         foreach (var dict in dictionariesToRemove)
         {
            dictionaries.Remove(dict);
         }

         try
         {
            // Добавляем новый словарь
            var dictionary = new ResourceDictionary
            {
               Source = new Uri($"Resources/Strings.{culture.Name}.xaml", UriKind.Relative)
            };
            dictionaries.Add(dictionary);
         }
         catch (Exception ex)
         {
            Console.WriteLine($"Error loading localization: {ex.Message}");
            // Если файл не найден, используем резервный (например, русский)
            var fallbackDictionary = new ResourceDictionary
            {
               Source = new Uri("Resources/Strings.ru-RU.xaml", UriKind.Relative)
            };
            dictionaries.Add(fallbackDictionary);
         }
      });
   }
}