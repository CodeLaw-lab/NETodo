using System.IO;
using System.Text.Json;
using TodoApp.WPF.Core.Interfaces;
using TodoApp.WPF.Models;

namespace TodoApp.WPF.Services;

public class ConfigurationService : IConfigurationService
{
   private const string SettingsFileName = "appsettings.user.json";
   private readonly string _settingsPath;

   public AppSettings AppSettings { get; private set; }

   public ConfigurationService()
   {
      var appDataPath = Path.Combine(
         Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TodoApp");

      Directory.CreateDirectory(appDataPath);
      _settingsPath = Path.Combine(appDataPath, SettingsFileName);
      AppSettings = new AppSettings();
   }

   public async Task LoadAsync()
   {
      try
      {
         if (File.Exists(_settingsPath))
         {
            var json = await File.ReadAllTextAsync(_settingsPath);
            AppSettings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
         }
         else
         {
            AppSettings = new AppSettings();
            await SaveAsync();
         }
      }
      catch
      {
         AppSettings = new AppSettings();
      }
   }

   public async Task SaveAsync()
   {
      try
      {
         var options =new JsonSerializerOptions { WriteIndented = true };
         var json = JsonSerializer.Serialize(AppSettings, options);
         await File.WriteAllTextAsync(_settingsPath, json);
      }
      catch (Exception ex)
      {
         Console.WriteLine($"Error saving settings: {ex.Message}");
      }
   }
}