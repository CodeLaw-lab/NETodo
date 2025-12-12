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
         ShowErrorMessage(ex.Message, "Ошибка");
      }
      finally
      {
         IsBusy = false;
      }
   }

   protected void ShowMessage(string message, string caption = "Сообщение")
   {
      Application.Current.Dispatcher.Invoke(() =>
      {
         MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
      });
   }

   protected void ShowErrorMessage(string message, string caption = "Ошибка")
   {
      Application.Current.Dispatcher.Invoke(() =>
      {
         MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
      });
   }

   protected MessageBoxResult ShowConfirmation(string message, string caption = "Подтверждение")
   {
      return Application.Current.Dispatcher.Invoke(() =>
      {
         return MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
      });
   }
}