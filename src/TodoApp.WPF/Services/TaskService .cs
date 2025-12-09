using TodoApp.WPF.Core.Entities;
using TodoApp.WPF.Core.Enums;
using TodoApp.WPF.Core.Interfaces;

namespace TodoApp.WPF.Services;

public class TaskService : ITaskService
{
   private readonly ITaskRepository _taskRepository;

   public TaskService(ITaskRepository taskRepository)
   {
      _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
   }

   public async Task<TodoTask> CreateTaskAsync(TodoTask task)
   {
      ValidateTask(task);

      // Автоматически устанавливаем CreatedAt и ModifiedAt
      task.CreatedAt = DateTime.UtcNow;
      task.ModifiedAt = DateTime.UtcNow;
      task.IsDeleted = false;

      return await _taskRepository.AddAsync(task);
   }

   public async Task DeleteTaskAsync(int id)
   {
      if (id <= 0)
      {
         throw new ArgumentException("Invalid task ID", nameof(id));
      }

      var task = await _taskRepository.GetByIdAsync(id);
      if (task == null)
      {
         throw new KeyNotFoundException($"Task with ID {id} nat found");
      }

      await _taskRepository.DeleteAsync(id);
   }

   public async Task<IEnumerable<TodoTask>> GetAllTasksAsync()
   {
      return await _taskRepository.GetAllAsync();
   }

   public async Task<TodoTask?> GetTaskByIdAsync(int id)
   {
      if (id <= 0)
         throw new ArgumentException("Invalid task ID", nameof(id));

      return await _taskRepository.GetByIdAsync(id);
   }

   public async Task<IEnumerable<TodoTask>> GetTasksByCategoryAsync(int categoryId)
   {
      if (categoryId <= 0)
         throw new ArgumentException("Invalid category ID", nameof(categoryId));

      return await _taskRepository.GetTasksByCategoryAsync(categoryId);
   }

   public async Task ToggleTaskCompletionAsync(int id)
   {
      if (id < 0)
         throw new ArgumentException("Invalid task ID", nameof(id));

      var task = await _taskRepository.GetByIdAsync(id);
      if (task == null)
         throw new KeyNotFoundException($"Task with ID {id} not found");

      task.IsCompleted = !task.IsCompleted;
      task.ModifiedAt = DateTime.UtcNow;

      await _taskRepository.UpdateAsync(task);
   }

   public async Task UpdateTaskAsync(TodoTask task)
   {
      if (task == null)
         throw new ArgumentNullException(nameof(task));

      ValidateTask(task);

      var existingTask = await _taskRepository.GetByIdAsync(task.Id);
      if (existingTask == null)
         throw new KeyNotFoundException($"Task with ID {task.Id} not found");

      // Обновляем поля, сохраняя некоторые системные
      existingTask.Title = task.Title;
      existingTask.Description = task.Description;
      existingTask.DueDate = task.DueDate;
      existingTask.Priority = task.Priority;
      existingTask.CategoryId = task.CategoryId;
      existingTask.ModifiedAt = DateTime.UtcNow;

      await _taskRepository.UpdateAsync(existingTask);
   }

   private void ValidateTask(TodoTask task)
   {
      if (task == null)
         throw new ArgumentNullException(nameof(task));

      if (string.IsNullOrWhiteSpace(task.Title))
         throw new ArgumentException("Task title is required", nameof(task.Title));

      if (task.Title.Length < 3)
         throw new ArgumentException("Task title must be at least 3 characters long", nameof(task.Title));

      if (task.Title.Length > 200)
         throw new ArgumentException("Task title must not exceed 200 characters", nameof(task.Title));

      if (task.Description?.Length > 2000)
         throw new ArgumentException("Task description cannot exceed 2000 characters", nameof(task.Description));

      // Проверка даты выполнения (не может быть в прошлом)
      if (task.DueDate.HasValue && task.DueDate.Value < DateTime.UtcNow.Date)
         throw new ArgumentException("Due date cannot be in the past", nameof(task.DueDate));
   }

   public async Task<IEnumerable<TodoTask>> GetTasksByPriorityAsync(Priority priority)
   {
      return await _taskRepository.FindAsync(t => t.Priority == priority);
   }

   public async Task<IEnumerable<TodoTask>> GetOverdueTasksAsync()
   {
      return await _taskRepository.FindAsync(t =>
         !t.IsCompleted &&
         t.DueDate.HasValue &&
         t.DueDate.Value < DateTime.UtcNow);
   }

   public async Task<IEnumerable<TodoTask>> GetCompletedTasksAsync()
   {
      return await _taskRepository.FindAsync(t => t.IsCompleted);
   }

   public async Task<IEnumerable<TodoTask>> GetPendingTasksAsync()
   {
      return await _taskRepository.FindAsync(t => !t.IsCompleted);
   }

   public async Task<(IEnumerable<TodoTask> Task, int TotalCount)> GetTasksWithPaginationAsync(
      int pageNumber,
      int pageSize,
      Func<TodoTask, bool>? filter = null)
   {
      var allTasks = await GetAllTasksAsync();

      if (filter != null)
         allTasks = allTasks.Where(filter);

      var totalCount = allTasks.Count();
      var tasks = allTasks
         .Skip((pageNumber - 1) * pageSize)
         .Take(pageSize);

      return (tasks, totalCount);
   }
}