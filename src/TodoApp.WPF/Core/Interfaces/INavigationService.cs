namespace TodoApp.WPF.Core.Interfaces;

public interface INavigationService
{
   Task<bool?> ShowTaskEditDialogAsync(int? taskId = null);
   Task ShowCategoryManagerDialogAsync();
   Task<bool?> ShowSettingsDialogAsync();
   void CloseDialog();
}