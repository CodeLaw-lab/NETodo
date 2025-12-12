using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoApp.WPF.Core.Entities;
using TodoApp.WPF.Core.Enums;
using TodoApp.WPF.Core.Interfaces;

namespace TodoApp.WPF.ViewModels;

public partial class TaskEditViewModel : ViewModelBase
{
   private readonly ITaskService _taskService;
   private readonly ICategoryService _categoryService;
   private readonly IDialogService _dialogService;
   private readonly INavigationService _navigationService;
   private readonly bool _isEditMode;
   private readonly int _taskId;

   public TaskEditViewModel(
       ITaskService taskService,
       ICategoryService categoryService,
       IDialogService dialogService,
       INavigationService navigationService,
       int? taskId = null)
   {
       _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
       _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
       _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
       _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
       
       _isEditMode = taskId.HasValue;
       _taskId = taskId ?? 0;

       Title = _isEditMode ? "Редактирование задачи" : "Новая задача";
       SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
       CancelCommand = new RelayCommand(Cancel);
       LoadCategoriesCommand = new AsyncRelayCommand(LoadCategoriesAsync);

       // Загрузка данных
       LoadCategoriesCommand.Execute(null);

       if (_isEditMode)
       {
          LoadTaskCommand = new AsyncRelayCommand(LoadTaskAsync);
          LoadTaskCommand.Execute(null);
       }
   }

   [ObservableProperty] private string _title;
   [ObservableProperty] private string _taskTitle = string.Empty;
   [ObservableProperty] private string? _description;
   [ObservableProperty] private DateTime _dueDate = DateTime.Today.AddDays(1);
   [ObservableProperty] private Priority _priority = Priority.Medium;
   [ObservableProperty] private Category? _selectedCategory;

   public ObservableCollection<Category> Categories { get; } = new();
   public ObservableCollection<Priority> AvailablePriorities { get; } = new()
   {
       Priority.Low,
       Priority.Medium,
       Priority.High,
       Priority.Critical
   };

   public IAsyncRelayCommand SaveCommand { get; }
   public IRelayCommand CancelCommand { get; }
   public IAsyncRelayCommand LoadCategoriesCommand { get; }
   public IAsyncRelayCommand? LoadTaskCommand { get; }

   private bool CanSave() =>
      !string.IsNullOrWhiteSpace(TaskTitle) && 
      TaskTitle.Length >= 3;

   private async Task SaveAsync()
   {
      await ExecuteAsync(async () =>
      {
         var task = new TodoTask
         {
            Id = _isEditMode ? _taskId : 0,
            Title = TaskTitle,
            Description = Description,
            DueDate = DueDate,
            Priority = Priority,
            CategoryId = SelectedCategory?.Id
         };

         if (_isEditMode)
         {
            await _taskService.UpdateTaskAsync(task);
            await _dialogService.ShowMessageAsync("Задача успешно обновлена", "Успех");
         }
         else
         {
            await _taskService.CreateTaskAsync(task);
            await _dialogService.ShowMessageAsync("Задача успешно создана", "Успех");
         }

         _navigationService.CloseDialog();
      });
   }

   private void Cancel()
   {
      _navigationService.CloseDialog();
   }

   private async Task LoadCategoriesAsync()
   {
      await ExecuteAsync(async () =>
      {
         var categories = await _categoryService.GetAllCategoriesAsync();
         Categories.Clear();
         foreach (var category in categories.OrderBy(c => c.Name))
         {
            Categories.Add(category);
         }

         // Выбираем первую категорию по умолчанию
         if (Categories.Any() && SelectedCategory == null)
         {
            SelectedCategory = Categories.First();
         }
      });
   }

   private async Task LoadTaskAsync()
   {
      if (_taskId <= 0) return;

      await ExecuteAsync(async () =>
      {
         var task = await _taskService.GetTaskByIdAsync(_taskId);
         if (task != null)
         {
            TaskTitle = task.Title;
            Description = task.Description;
            DueDate = (DateTime)task.DueDate;
            Priority = task.Priority;

            if (task.CategoryId.HasValue)
            {
               var categories = await _categoryService.GetAllCategoriesAsync();
               SelectedCategory = categories.FirstOrDefault(c => c.Id == task.CategoryId.Value);
            }
         }
      });
   }
}