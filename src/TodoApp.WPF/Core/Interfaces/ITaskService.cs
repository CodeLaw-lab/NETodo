using TodoApp.WPF.Core.Entities;

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
}