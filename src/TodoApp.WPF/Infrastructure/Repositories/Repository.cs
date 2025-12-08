using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TodoApp.WPF.Core.Entities;
using TodoApp.WPF.Core.Interfaces;
using TodoApp.WPF.Infrastructure.Data;

namespace TodoApp.WPF.Infrastructure.Repositories
{
   public class Repository<T> : IRepository<T> where T : EntityBase
   {
      protected readonly AppDbContext _context;
      protected readonly DbSet<T> _dbSet;

      public Repository(AppDbContext context)
      {
         _context = context;
         _dbSet = context.Set<T>();
      }

      public virtual async Task<T?> GetByIdAsync(int id)
      {
         return await _dbSet.FindAsync(id);
      }

      public virtual async Task<IEnumerable<T>> GetAllAsync()
      {
         return await _dbSet.Where(e => !e.IsDeleted).ToListAsync();
      }

      public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
      {
         return await _dbSet.Where(e => !e.IsDeleted).Where(predicate).ToListAsync();
      }

      public virtual async Task<T> AddAsync(T entity)
      {
         await _dbSet.AddAsync(entity);
         await _context.SaveChangesAsync();
         return entity;
      }

      public virtual async Task UpdateAsync(T entity)
      {
         entity.ModifiedAt = DateTime.UtcNow;
         _dbSet.Update(entity);
         await _context.SaveChangesAsync();
      }

      public virtual async Task DeleteAsync(int id)
      {
         var entity = await GetByIdAsync(id);
         if (entity != null)
         {
            entity.IsDeleted = true;
            entity.ModifiedAt = DateTime.UtcNow;
            await UpdateAsync(entity);
         }
      }

      public virtual async Task<bool> ExistsAsync(int id)
      {
         return await _dbSet.AnyAsync(e => e.Id == id && !e.IsDeleted);
      }
   }
}