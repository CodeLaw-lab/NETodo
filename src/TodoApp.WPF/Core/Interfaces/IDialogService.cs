namespace TodoApp.WPF.Core.Interfaces;

public interface IDialogService
{
   Task ShowMessageAsync(string message, string caption);
   Task<bool> ShowConfirmationAsync(string message, string caption);
   Task<string?> ShowInputDialogAsync(string message, string caption, string defaultValue = "");
   Task<string?> ShowFileDialogAsync(bool save, string filter, string defaultName = "");
}