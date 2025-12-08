using TodoApp.WPF.Core.Entities;

namespace TodoApp.WPF.Core.Interfaces;

public interface ITaskRepository : IRepository<TodoTask>
{
   Task<IEnumerable<TodoTask>> GetTasksByCategoryAsync(int categoryId);
}