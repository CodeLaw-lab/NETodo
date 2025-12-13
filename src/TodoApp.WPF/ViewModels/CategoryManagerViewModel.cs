using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoApp.WPF.Core.Entities;
using TodoApp.WPF.Core.Interfaces;

namespace TodoApp.WPF.ViewModels;

public partial class CategoryManagerViewModel : ViewModelBase
{
   private readonly ICategoryService _categoryService;
   private readonly IDialogService _dialogService;
   private readonly INavigationService _navigationService;
   private Category? _selectedCategory;

   public CategoryManagerViewModel(
      ICategoryService categoryService,
      IDialogService dialogService,
      INavigationService navigationService)
   {
      _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
      _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
      _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

      LoadCategoriesCommand = new AsyncRelayCommand(LoadCategoriesAsync);
      AddCategoryCommand = new AsyncRelayCommand(AddCategoryAsync);
      EditCategoryCommand = new AsyncRelayCommand(EditCategoryAsync, CanEditCategory);
      DeleteCategoryCommand = new AsyncRelayCommand(DeleteCategoryAsync, CanDeleteCategory);
      SaveCategoryCommand = new AsyncRelayCommand(SaveCategoryAsync, CanSaveCategory);
      CancelEditCommand = new RelayCommand(CancelEdit);
      CloseCommand = new RelayCommand(Close);

      LoadCategoriesCommand.Execute(null);
   }

   [ObservableProperty] private string _title = "Управление категориями";
   [ObservableProperty] private bool _isEditing;
   [ObservableProperty] private string _categoryName = string.Empty;
   [ObservableProperty] private string _categoryColor = "#007ACC";
   [ObservableProperty] private string _categoryIcon = "FolderOutline";

   public ObservableCollection<Category> Categories { get; } = new();

   public ObservableCollection<string> AvailableIcons { get; } = new()
   {
      "📁", "📂", "📅", "⭐", "🏷️", "📍", "🔖", "📌", "📎", "🔗"
   };

   public Category? SelectedCategory
   {
      get => _selectedCategory;
      set
      {
         if (SetProperty(ref _selectedCategory, value))
         {
            EditCategoryCommand.NotifyCanExecuteChanged();
            DeleteCategoryCommand.NotifyCanExecuteChanged();
         }
      }
   }

   public IAsyncRelayCommand LoadCategoriesCommand { get; }
   public IAsyncRelayCommand AddCategoryCommand { get; }
   public IAsyncRelayCommand EditCategoryCommand { get; }
   public IAsyncRelayCommand DeleteCategoryCommand { get; }
   public IAsyncRelayCommand SaveCategoryCommand { get; }
   public IRelayCommand CancelEditCommand { get; }
   public IRelayCommand CloseCommand { get; }

   private async Task LoadCategoriesAsync()
   {
      await ExecuteAsync(async () =>
      {
         var categories = await _categoryService.GetAllCategoriesAsync();
         Categories.Clear();
         foreach (var category in categories.OrderBy(c => c.Name))
         {
            Categories.Add(category);
         }
      });
   }

   private async Task AddCategoryAsync()
   {
      ResetForm();
      IsEditing = true;
   }

   private bool CanEditCategory() => SelectedCategory != null && !IsEditing;

   private async Task EditCategoryAsync()
   {
      if (SelectedCategory == null) return;

      CategoryName = SelectedCategory.Name;
      CategoryColor = SelectedCategory.Color;
      CategoryIcon = SelectedCategory.Icon;
      IsEditing = true;
   }

   private bool CanDeleteCategory() => SelectedCategory != null && !IsEditing;

   private async Task DeleteCategoryAsync()
   {
      if (SelectedCategory == null) return;

      var hasTasks = await _categoryService.CategoryHasTasksAsync(SelectedCategory.Id);
      if (hasTasks)
      {
         await _dialogService.ShowMessageAsync(
            $"Ревозможно удалить категорию '{SelectedCategory.Name}', так как в ней есть задачи",
            "Ошибка удаления");
         return;
      }

      var confirm = await _dialogService.ShowConfirmationAsync(
         $"Вы уверены, что хотите удалить категорию '{SelectedCategory.Name}'?",
         "Подтверждение удаления");

      if (confirm)
      {
         await ExecuteAsync(async () =>
         {
            await _categoryService.DeleteCategoryAsync(SelectedCategory.Id);
            Categories.Remove(SelectedCategory);
            SelectedCategory = null;
            StatusMessage = $"Категория удалена";
         });
      }
   }

   private bool CanSaveCategory() =>
      !string.IsNullOrWhiteSpace(CategoryName) &&
      CategoryName.Length >= 2 &&
      CategoryName.Length <= 100;

   private async Task SaveCategoryAsync()
   {
      await ExecuteAsync(async () =>
      {
         var category = new Category
         {
            Id = SelectedCategory?.Id ?? 0,
            Name = CategoryName.Trim(),
            Color = CategoryColor,
            Icon = CategoryIcon
         };

         if (SelectedCategory == null)
         {
            // Добавление новой категории
            var createdCategoty = await _categoryService.CreateCategoryAsync(category);
            Categories.Add(createdCategoty);
            StatusMessage = $"Категория '{CategoryName}' создана";
         }
         else
         {
            // Обновление существующей категории
            await _categoryService.UpdateCategoryAsync(category);

            // Обновляем в коллекции
            var index = Categories.IndexOf(SelectedCategory);
            if (index >= 0)
            {
               Categories[index] = category;
            }

            StatusMessage = $"Категория '{CategoryName}' обновлена";
         }

         CancelEdit();
      });
   }

   private void CancelEdit()
   {
      IsEditing = false;
      ResetForm();
   }

   private void Close()
   {
      _navigationService.CloseDialog();
   }

   private void ResetForm()
   {
      CategoryName = string.Empty;
      CategoryColor = "#007ACC";
      CategoryIcon = "📁";
      SelectedCategory = null;
   }
}