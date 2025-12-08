using Microsoft.EntityFrameworkCore;

using TodoApp.WPF.Core.Entities;
using TodoApp.WPF.Core.Interfaces;
using TodoApp.WPF.Infrastructure.Data;

namespace TodoApp.WPF.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
   public CategoryRepository(AppDbContext context) : base(context)
   {
   }

   public new async Task<IEnumerable<Category>> GetAllAsync()
   {
      return await _context.Categories
         .Include(c => c.Tasks)
         .ToListAsync();
   }
}