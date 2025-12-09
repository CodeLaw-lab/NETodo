using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TodoApp.WPF.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
   private bool _isBusy;
   private string _statusMessage = string.Empty;

   public bool IsBusy
   {
      get => _isBusy;
      set => SetProperty(ref _isBusy, value);
   }

   public string StatusMessage
   {
      get => _statusMessage;
      set => SetProperty(ref _statusMessage, value);
   }

   protected async Task ExecuteAsync(Func<Task> operation, Action<Exception>? onError = null)
   {
      try
      {
         IsBusy = true;
         StatusMessage = "Выполнение операции...";
         await operation();
         StatusMessage = "Операция завершена успешно";
      }
      catch (Exception ex)
      {
         StatusMessage = $"Ошибка: {ex.Message}";
         onError?.Invoke(ex);

         // Показываем сообщение об ошибке пользователю
         Application.Current.Dispatcher.Invoke(() =>
         {
            MessageBox.Show(
               $"Произошла ошибка: {ex.Message}",
               "Ошибка",
               MessageBoxButton.OK,
               MessageBoxImage.Error);
         });
      }
      finally
      {
         IsBusy = false;
      }
   }
}