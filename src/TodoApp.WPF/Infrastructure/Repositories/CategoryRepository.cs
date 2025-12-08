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
         .Include(c => c.Tasks.Where(t => !t.IsDeleted))
         .Where(c => !c.IsDeleted)
         .ToListAsync();
   }

   public new async Task<Category?> GetByIdAsync(int id)
   {
      return await _context.Categories
         .Include(c => c.Tasks.Where(t => !t.IsDeleted))
         .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
   }

   public new async Task<IEnumerable<Category>> FindAsync(System.Linq.Expressions.Expression<Func<Category, bool>> predicate)
   {
      return await _context.Categories
         .Include(c => c.Tasks.Where(t => !t.IsDeleted))
         .Where(c => !c.IsDeleted)
         .Where(predicate)
         .ToListAsync();
   }
}