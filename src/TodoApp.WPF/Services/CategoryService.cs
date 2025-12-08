using TodoApp.WPF.Core.Entities;
using TodoApp.WPF.Core.Interfaces;

namespace TodoApp.WPF.Services;

public class CategoryService : ICategoryService
{
   private readonly ICategoryRepository _categoryRepository;
   private readonly ITaskRepository _taskRepository;

   public CategoryService(ICategoryRepository categoryRepository, ITaskRepository taskRepository)
   {
      _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
      _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
   }

   public async Task<Category> CreateCategoryAsync(Category category)
   {
      ValidateCategory(category);

      // Проверка уникальности имени категории
      var existingCategory = (await _categoryRepository.FindAsync(c => c.Name == category.Name)).FirstOrDefault();
      if (existingCategory != null && !existingCategory.IsDeleted)
         throw new InvalidOperationException($"Category with name '{category.Name}' already exists");

      category.CreatedAt = DateTime.UtcNow;
      category.ModifiedAt = DateTime.UtcNow;
      category.IsDeleted = false;

      return await _categoryRepository.AddAsync(category);
   }

   public async Task DeleteCategoryAsync(int id)
   {
      if (id <= 0)
         throw new ArgumentException("Invalid category ID", nameof(id));

      var category = await _categoryRepository.GetByIdAsync(id);
      if (category == null)
         throw new KeyNotFoundException($"Category with ID {id} not found");

      // Проверяем, есть ли связанные задачи
      var tasks = await _taskRepository.GetTasksByCategoryAsync(id);
      var activeTasks = tasks.Where(t => !t.IsDeleted).ToList();

      if (activeTasks.Any())
         throw new InvalidOperationException($"Cannot delete category '{category.Name}' because it has {activeTasks.Count} active task(s).");

      await _categoryRepository.DeleteAsync(id);
   }

   public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
   {
      return await _categoryRepository.GetAllAsync();
   }

   public async Task<Category?> GetCategoryByIdAsync(int id)
   {
      if (id <= 0)
         throw new ArgumentException("Invalid category ID", nameof(id));

      return await _categoryRepository.GetByIdAsync(id);
   }

   public async Task UpdateCategoryAsync(Category category)
   {
      if (category == null)
         throw new ArgumentNullException(nameof(category));

      ValidateCategory(category);

      var existingCategory = await _categoryRepository.GetByIdAsync(category.Id);
      if (existingCategory == null)
         throw new KeyNotFoundException($"Category with ID {category.Id} not found");

      // Проверка уникальности имени (если имя изменилось)
      if (existingCategory.Name != category.Name)
      {
         var duplicateCategory = (await _categoryRepository.FindAsync(c => c.Name == category.Name)).FirstOrDefault();
         if (duplicateCategory != null && !duplicateCategory.IsDeleted)
            throw new InvalidOperationException($"Category with name '{category.Name}' already exists");
      }

      existingCategory.Name = category.Name;
      existingCategory.Color = category.Color;
      existingCategory.Icon = category.Icon;
      existingCategory.ModifiedAt = DateTime.UtcNow;

      await _categoryRepository.UpdateAsync(existingCategory);
   }

   private void ValidateCategory(Category category)
   {
      if (category == null)
         throw new ArgumentNullException(nameof(category));

      if (string.IsNullOrWhiteSpace(category.Name))
         throw new ArgumentException("Category name is required", nameof(category.Name));

      if (category.Name.Length < 2)
         throw new ArgumentException("Category name must be at least 2 characters long", nameof(category.Name));

      if (category.Name.Length > 100)
         throw new ArgumentException("Category name cannot exceed 100 characters", nameof(category.Name));

      if (string.IsNullOrWhiteSpace(category.Color))
         throw new ArgumentException("Category color is required", nameof(category.Color));

      // Проверка формата цвета HEX
      if (!System.Text.RegularExpressions.Regex.IsMatch(category.Color, "^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$"))
         throw new ArgumentException("Category color must be a valid HEX color (e.g., #RRGGBB)", nameof(category.Color));

      if (string.IsNullOrWhiteSpace(category.Icon))
         throw new ArgumentException("Category icon is required", nameof(category.Icon));

      if (category.Icon.Length > 50)
         throw new ArgumentException("Category icon cannot exceed 50 characters", nameof(category.Icon));
   }

   public async Task<Category?> GetCategoryByNameAsync(string name)
   {
      if (string.IsNullOrWhiteSpace(name))
         throw new ArgumentException("Category name cannot be empty", nameof(name));

      var categories = await _categoryRepository.FindAsync(c => c.Name == name);
      return categories.FirstOrDefault(c => !c.IsDeleted);
   }

   public async Task<bool> CategoryHasTasksAsync(int categoryId)
   {
      var tasks = await _taskRepository.GetTasksByCategoryAsync(categoryId);
      return tasks.Any(t => !t.IsDeleted);
   }
}