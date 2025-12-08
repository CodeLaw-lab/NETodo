using System.Linq.Expressions;
using TodoApp.WPF.Core.Entities;

namespace TodoApp.WPF.Core.Interfaces;

public interface IRepository<T> where T : EntityBase
{
   Task<T?> GetByIdAsync(int id);
   Task<IEnumerable<T>> GetAllAsync();
   Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
   Task<T> AddAsync(T entity);
   Task UpdateAsync(T entity);
   Task DeleteAsync(int id);
   Task<bool> ExistsAsync(int id);
}