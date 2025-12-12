using System.Windows;
using Microsoft.Win32;
using TodoApp.WPF.Core.Interfaces;

namespace TodoApp.WPF.Services;

public class DialogService : IDialogService
{
   public async Task ShowMessageAsync(string message, string caption)
   {
      await Application.Current.Dispatcher.InvokeAsync(() =>
      {
         MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
      });
   }

   public async Task<bool> ShowConfirmationAsync(string message, string caption)
   {
      return await Application.Current.Dispatcher.InvokeAsync(() =>
      {
         var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
         return result == MessageBoxResult.Yes;
      });
   }

   public async Task<string?> ShowInputDialogAsync(string message, string caption, string defaultValue = "")
   {
      // Упрощеннаяреализация - используем MessageBox с TaxtBox
      return await Task.Run(() =>
      {
         // TODO: Реализовать собственное окно ввода
         return defaultValue;
      });
   }

   public async Task<String?> ShowFileDialogAsync(bool save, string filter, string defaultName = "")
   {
      return await Task.Run(() =>
      {
         if (save)
         {
            var saveDialog = new SaveFileDialog
            {
               Filter = filter,
               FileName = defaultName,
               DefaultExt = ".json"
            };
            return saveDialog.ShowDialog() == true ? saveDialog.FileName : null;
         }
         else
         {
            var openDialog = new OpenFileDialog
            {
               Filter = filter,
               DefaultExt = ".json"
            };
            return openDialog.ShowDialog() == true ? openDialog.FileName : null;
         }
      });
   }
}