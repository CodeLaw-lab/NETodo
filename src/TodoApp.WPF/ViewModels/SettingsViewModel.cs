using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoApp.WPF.Core.Interfaces;

namespace TodoApp.WPF.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
   private readonly IThemeService _themeService;
   private readonly ILocalizationService _localizationService;
   private readonly INavigationService _navigationService;
   private readonly IDialogService _dialogService;

   public SettingsViewModel(
      IThemeService themeService,
      ILocalizationService localizationService,
      INavigationService navigationService)
   {
      _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
      _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
      _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

      SaveCommand = new AsyncRelayCommand(SaveAsync);
      CancelCommand = new RelayCommand(Cancel);
      CloseCommand = new RelayCommand(Close);

      LoadSettings();
   }

   [ObservableProperty] private string _title = "Настройки";
   [ObservableProperty] private string _selectedTheme;
   [ObservableProperty] private CultureInfo? _selectedCulture;

   public ObservableCollection<string> AvailableThemes { get; } = new();
   public ObservableCollection<CultureInfo> AvailableCultures { get; } = new();

   public IAsyncRelayCommand SaveCommand { get; }
   public IRelayCommand CancelCommand { get; }
   public IRelayCommand CloseCommand { get; }

   private void LoadSettings()
   {
      // Загрузка доступных тем
      AvailableThemes.Clear();
      foreach (var theme in _themeService.GetAvailableThemes())
      {
         AvailableThemes.Add(theme);
      }
      SelectedTheme = _themeService.GetCurrentTheme();

      // Загрузка доступных языков
      AvailableCultures.Clear();
      foreach (var culture in _localizationService.GetAvailableCultures())
      {
         AvailableCultures.Add(culture);
      }
      SelectedCulture = _localizationService.GetCurrentCulture();
   }

   private async Task SaveAsync()
   {
      await ExecuteAsync(async () =>
      {
         if (!string.IsNullOrEmpty(SelectedTheme))
         {
            await _themeService.SetThemeAsync(SelectedTheme);
         }

         if (_selectedCulture != null)
         {
            await _localizationService.SetCultureAsync(SelectedCulture);
         }

         await _dialogService.ShowMessageAsync("Настройки сохранены.", "Успех");
         Close();
      });
   }

   private void Cancel()
   {
      Close();
   }

   private void Close()
   {
      _navigationService.CloseDialog();
   }
}