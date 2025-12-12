using System.Windows;
using TodoApp.WPF.Core.Interfaces;
using TodoApp.WPF.ViewModels;
using TodoApp.WPF.Views;

namespace TodoApp.WPF.Services;

public class NavigationService : INavigationService
{
   private readonly IServiceProvider _serviceProvider;

   public NavigationService(IServiceProvider serviceProvider)
   {
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
   }

   public async Task<bool?> ShowTaskEditDialogAsync(int? taskId = null)
   {
      return await Application.Current.Dispatcher.InvokeAsync(() =>
      {
         var viewModel = _serviceProvider.GetService(typeof(TaskEditViewModel)) as TaskEditViewModel;
         if (viewModel == null)
         {
            MessageBox.Show("Ошибка создания окна редактирования", "Ошибка",
               MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
         }

         var window = new TaskEditWindow(viewModel);
         return window.ShowDialog();
      });
   }

   public async Task ShowCategoryManagerDialogAsync()
   {
      await Application.Current.Dispatcher.InvokeAsync(() =>
      {
         MessageBox.Show("Управление категориями", "В разработке",
            MessageBoxButton.OK, MessageBoxImage.Information);
      });
   }

   public async Task<bool?> ShowSettingsDialogAsync()
   {
      await Application.Current.Dispatcher.InvokeAsync(() =>
      {
         MessageBox.Show("Настройки", "В разработке",
            MessageBoxButton.OK, MessageBoxImage.Information);
      });

      return null;
   }

   public void CloseDialog()
   {
      // Найдем и закроем активное диалоговое окно
      Application.Current.Dispatcher.Invoke(() =>
      {
         var activeWindow = Application.Current.Windows.OfType<Window>()
            .FirstOrDefault(w => w.Owner != null);
         activeWindow?.Close();
      });
   }
}