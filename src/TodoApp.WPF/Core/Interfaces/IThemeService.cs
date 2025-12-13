namespace TodoApp.WPF.Core.Interfaces;

public interface IThemeService
{
   IEnumerable<string> GetAvailableThemes();
   string GetCurrentTheme();
   Task SetThemeAsync(string themeName);
   event EventHandler<string> ThemeChanged;
}