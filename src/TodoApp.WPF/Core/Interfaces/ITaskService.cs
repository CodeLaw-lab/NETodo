using TodoApp.WPF.Core.Entities;
using TodoApp.WPF.Core.Enums;

namespace TodoApp.WPF.Core.Interfaces;

public interface ITaskService
{
   Task<IEnumerable<TodoTask>> GetAllTasksAsync();
   Task<TodoTask?> GetTaskByIdAsync(int id);
   Task<TodoTask> CreateTaskAsync(TodoTask task);
   Task UpdateTaskAsync(TodoTask task);
   Task DeleteTaskAsync(int id);
   Task ToggleTaskCompletionAsync(int id);
   Task<IEnumerable<TodoTask>> GetTasksByCategoryAsync(int categoryId);

   // Дополнительные методы
   Task<IEnumerable<TodoTask>> GetTasksByPriorityAsync(Priority priority);
   Task<IEnumerable<TodoTask>> GetOverdueTasksAsync();
   Task<IEnumerable<TodoTask>> GetCompletedTasksAsync();
   Task<IEnumerable<TodoTask>> GetPendingTasksAsync();
}