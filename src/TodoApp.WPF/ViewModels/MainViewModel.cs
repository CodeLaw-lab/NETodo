using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoApp.WPF.Core.Entities;
using TodoApp.WPF.Core.Enums;
using TodoApp.WPF.Core.Interfaces;

namespace TodoApp.WPF.ViewModels;

public partial class MainViewModel : ViewModelBase
{
   private readonly ITaskService _taskService;
   private readonly ICategoryService _categoryService;
   private readonly INavigationService _navigationService;

   private TodoTask? _selectedTask;
   private Category? _selectedCategory;
   private string _searchText = string.Empty;
   private bool _showCompleted = true;
   private Priority? _selectedPriority;

   [ObservableProperty]
   private string _title = "TodoApp - Менеджер задач";

   public MainViewModel(ITaskService taskService, ICategoryService categoryService, INavigationService navigationService)
   {
      _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
      _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
      _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

      LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
      AddTaskCommand = new AsyncRelayCommand(AddTaskAsync);
      EditTaskCommand = new AsyncRelayCommand(EditTaskAsync, CanEditTask);
      DeleteTaskCommand = new AsyncRelayCommand(DeleteTaskAsync, CanDeleteTask);
      ToggleTaskCompletionCommand = new AsyncRelayCommand(ToggleTaskCompletionAsync, CanToggleCompletion);
      ManageCategoriesCommand = new AsyncRelayCommand(ManageCategoriesAsync);
      ClearFiltersCommand = new RelayCommand(ClearFilters);
      RefreshCommand = new AsyncRelayCommand(RefreshAsync);

      // Загрузка данных при создании ViewModel
      LoadDataCommand.Execute(null);
   }

   public ObservableCollection<TodoTask> Tasks { get; } = new();
   public ObservableCollection<Category> Categories { get; } = new();
   public ObservableCollection<Priority> AvailablePriorities { get; } = new()
   {
      Priority.Low,
      Priority.Medium,
      Priority.High,
      Priority.Critical
   };

   public TodoTask? SelectedTask
   {
      get => _selectedTask;
      set
      {
         if (SetProperty(ref _selectedTask, value))
         {
            // Обновляем состояние команд
            EditTaskCommand.NotifyCanExecuteChanged();
            DeleteTaskCommand.NotifyCanExecuteChanged();
            ToggleTaskCompletionCommand.NotifyCanExecuteChanged();
         }
      }
   }

   public Category? SelectedCategory
   {
      get => _selectedCategory;
      set
      {
         if (SetProperty(ref _selectedCategory, value))
         {
            ApplyFilters();
         }
      }
   }

   public string SearchText
   {
      get => _searchText;
      set
      {
         if (SetProperty(ref _searchText, value))
         {
            ApplyFilters();
         }
      }
   }

   public bool ShowCompleted
   {
      get => _showCompleted;
      set
      {
         if (SetProperty(ref _showCompleted, value))
         {
            ApplyFilters();
         }
      }
   }

   public Priority? SelectedPriority
   {
      get => _selectedPriority;
      set
      {
         if (SetProperty(ref _selectedPriority, value))
         {
            ApplyFilters();
         }
      }
   }

   // Команды - используем конкретные типы для доступа к NotifyCanExecuteChanged
   public IAsyncRelayCommand LoadDataCommand { get; }
   public IAsyncRelayCommand AddTaskCommand { get; }
   public IAsyncRelayCommand EditTaskCommand { get; }
   public IAsyncRelayCommand DeleteTaskCommand { get; }
   public IAsyncRelayCommand ToggleTaskCompletionCommand { get; }
   public IAsyncRelayCommand ManageCategoriesCommand { get; }
   public IRelayCommand ClearFiltersCommand { get; }
   public IAsyncRelayCommand RefreshCommand { get; }

   // Статистика
   [ObservableProperty]
   private int _totalTasks;

   [ObservableProperty]
   private int _completedTasks;

   [ObservableProperty]
   private int _pendingTasks;

   [ObservableProperty]
   private int _overdueTasks;

   private async Task LoadDataAsync()
   {
      await ExecuteAsync(async () =>
      {
         StatusMessage = "Загрузка данных...";

         // Загрузка задач
         var allTasks = await _taskService.GetAllTasksAsync();
         Tasks.Clear();
         foreach (var task in allTasks.OrderBy(t => t.DueDate ?? DateTime.MaxValue))
         {
            Tasks.Add(task);
         }

         // Загрузка категорий
         var categories = await _categoryService.GetAllCategoriesAsync();
         Categories.Clear();
         foreach (var category in categories.OrderBy(c => c.Name))
         {
            Categories.Add(category);
         }

         UpdateStatistics();
         StatusMessage = $"Загружено {Tasks.Count} задач, {Categories.Count} категорий";
      });
   }

   private async Task AddTaskAsync()
   {
      var result = await _navigationService.ShowTaskEditDialogAsync();
      if (result == true)
      {
         await RefreshAsync();
         ShowMessage("Задача успешно создана", "Успех");
      }
      
   }

   private bool CanEditTask() => SelectedTask != null;

   private async Task EditTaskAsync()
   {
      if (SelectedTask == null) return;

      var result = await _navigationService.ShowTaskEditDialogAsync(SelectedTask.Id);
      if (result == true)
      {
         await RefreshAsync();
         ShowMessage("Задача успешно обновлена", "Успех");
      }
   }

   private bool CanDeleteTask() => SelectedTask != null;

   private async Task DeleteTaskAsync()
   {
      if (SelectedTask == null) return;

      var result = ShowConfirmation(
         $"Вы уверены, что хотите удалить задачу \"{SelectedTask.Title}\"?",
         "Подтверждение удаления");

      if (result == MessageBoxResult.Yes)
      {
         await ExecuteAsync(async () =>
         {
            await _taskService.DeleteTaskAsync(SelectedTask.Id);
            Tasks.Remove(SelectedTask);
            UpdateStatistics();
            StatusMessage = $"Задача \"{SelectedTask.Title}\" удалена";
         });
      }
   }

   private bool CanToggleCompletion() => SelectedTask != null;

   private async Task ToggleTaskCompletionAsync()
   {
      if (SelectedTask == null) return;

      await ExecuteAsync(async () =>
      {
         await _taskService.ToggleTaskCompletionAsync(SelectedTask.Id);
         SelectedTask.IsCompleted = !SelectedTask.IsCompleted;
         SelectedTask.ModifiedAt = DateTime.UtcNow;

         // Обновляем коллекцию для обновления UI
         Tasks.Remove(SelectedTask);
         Tasks.Add(SelectedTask);
         UpdateStatistics();

         StatusMessage = SelectedTask.IsCompleted
            ? $"Задача \"{SelectedTask.Title}\" отмечена как выполненная"
            : $"Задача \"{SelectedTask.Title}\" снова активна";
      });
   }

   private async Task ManageCategoriesAsync()
   {
      await _navigationService.ShowCategoryManagerDialogAsync();
   }

   private void ClearFilters()
   {
      SelectedCategory = null;
      SearchText = string.Empty;
      ShowCompleted = true;
      SelectedPriority = null;
      StatusMessage = "Фильтры сброшены";
   }

   private async Task RefreshAsync()
   {
      await LoadDataAsync();
   }

   private void ApplyFilters()
   {
      // TODO: Реализовать фильтрацию при загрузке данных
      StatusMessage = "Фильтры применены (реализация в процессе)";
   }

   private void UpdateStatistics()
   {
      var allTasks = Tasks.ToList();

      TotalTasks = allTasks.Count;
      CompletedTasks = allTasks.Count(t => t.IsCompleted);
      PendingTasks = allTasks.Count(t => !t.IsCompleted);
      OverdueTasks = allTasks.Count(t => !t.IsCompleted && t.IsOverdue());
   }

   private void ShowMessage(string message, string caption)
   {
      System.Windows.Application.Current.Dispatcher.Invoke(() =>
      {
         MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
      });
   }

   private MessageBoxResult ShowMessageBox(string message, string caption,
      MessageBoxButton buttons, MessageBoxImage icon)
   {
      return System.Windows.Application.Current.Dispatcher.Invoke(() =>
      {
         return MessageBox.Show(message, caption, buttons, icon);
      });
   }
}