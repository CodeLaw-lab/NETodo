using TodoApp.WPF.Core.Entities;

namespace TodoApp.WPF.Core.Interfaces;

public interface ICategoryService
{
   Task<IEnumerable<Category>> GetAllCategoriesAsync();
   Task<Category?> GetCategoryByIdAsync(int id);
   Task<Category> CreateCategoryAsync(Category category);
   Task UpdateCategoryAsync(Category category);
   Task DeleteCategoryAsync(int id);

   // Дополнительные методы
   Task<Category?> GetCategoryByNameAsync(string name);
   Task<bool> CategoryHasTasksAsync(int categoryId);
}