using Microsoft.EntityFrameworkCore;
using TodoApp.WPF.Core.Entities;
using TodoApp.WPF.Core.Interfaces;
using TodoApp.WPF.Infrastructure.Data;

namespace TodoApp.WPF.Infrastructure.Repositories;

public class TaskRepository : Repository<TodoTask>, ITaskRepository
{
   public TaskRepository(AppDbContext context) : base(context)
   {
   }

   public async Task<IEnumerable<TodoTask>> GetTasksByCategoryAsync(int categoryId)
   {
      return await _context.Tasks
         .Where(t => t.CategoryId == categoryId)
         .ToListAsync();
   }

   public new async Task<IEnumerable<TodoTask>> GetAllAsync()
   {
      return await _context.Tasks
         .Include(t => t.Category)
         .Where(t => !t.IsDeleted)
         .ToListAsync();
   }

   public new async Task<TodoTask?> GetByIdAsync(int id)
   {
      return await _context.Tasks
         .Include(t => t.Category)
         .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
   }
}