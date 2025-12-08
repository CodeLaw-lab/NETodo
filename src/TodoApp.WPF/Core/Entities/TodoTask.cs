using System.ComponentModel;
using TodoApp.WPF.Core.Enums;

namespace TodoApp.WPF.Core.Entities;

public class TodoTask : EntityBase
{
   private Guid _uuid = Guid.NewGuid();
   private string _title = String.Empty;
   private string? _description;
   private DateTime? dueDate;
   private Priority _priority = Priority.Medium;
   private bool _isCompleted;
   private DateTime? _completedAt;
   private int? _categoryId;
   private Category? _category;

   public Guid Uuid
   {
      get => _uuid;
      set => SetField(ref _uuid, value, nameof(Uuid));
   }

   public string Title
   {
      get => _title;
      set => SetField(ref _title, value, nameof(Title));
   }

   public string? Description
   {
      get => _description;
      set => SetField(ref _description, value, nameof(Description));
   }

   public DateTime? DueDate
   {
      get => dueDate;
      set => SetField(ref dueDate, value, nameof(DueDate));
   }

   public Priority Priority
   {
      get => _priority;
      set => SetField(ref _priority, value, nameof(Priority));
   }

   public bool IsCompleted
   {
      get => _isCompleted;
      set
      {
         if (SetField(ref _isCompleted, value, nameof(IsCompleted)))
         {
            if (value)
            {
               CompletedAt = DateTime.UtcNow;
            }
            else
            {
               CompletedAt = null;
            }
         }
      }
   }

   public DateTime? CompletedAt
   {
      get => _completedAt;
      private set => SetField(ref _completedAt, value, nameof(CompletedAt));
   }

   public int? CategoryId
   {
      get => _categoryId;
      set => SetField(ref _categoryId, value, nameof(CategoryId));
   }

   public Category? Category
   {
      get => _category;
      set => SetField(ref _category, value, nameof(Category));
   }

   public void MarkAsCompleted()
   {
      IsCompleted = true;
   }

   public void MarkAsIncomplete()
   {
      IsCompleted = false;
   }

   public bool IsOverdue()
   {
      return !IsCompleted && DueDate.HasValue && DueDate.Value < DateTime.UtcNow;
   }

   public void Update(string title, string description)
   {
      Title = title;
      Description = description;
      ModifiedAt = DateTime.UtcNow;
   }
}