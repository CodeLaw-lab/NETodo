namespace TodoApp.WPF.Models;

public class AppSettings
{
   public string Theme { get; set; } = "Light";
   public string Culture { get; set; } = "ru-Ru";
   public bool AutoStartWindows { get; set; } = false;
   public bool CheckForUpdates { get; set; } = true;
}