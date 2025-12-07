using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.WPF.Core.Entities;

public abstract class EntityBase : INotifyPropertyChanged
{
   private int _id;
   [Key]
   public int Id
   {
      get => _id;
      set
      {
         _id = value;
         OnPropertyChanged(nameof(Id));
      }
   }

   private DateTime _createdAt = DateTime.UtcNow;
   public DateTime CreatedAt
   {
      get => _createdAt;
      set
      {
         _createdAt = value;
         OnPropertyChanged(nameof(CreatedAt));
      }
   }

   private DateTime _modifiedAt = DateTime.UtcNow;
   public DateTime ModifiedAt
   {
      get => _modifiedAt;
      set
      {
         _modifiedAt = value;
         OnPropertyChanged(nameof(ModifiedAt));
      }
   }

   private bool _isDeleted;
   public bool IsDeleted
   {
      get => _isDeleted;
      set
      {
         _isDeleted = value;
         OnPropertyChanged(nameof(IsDeleted));
      }
   }

   public event PropertyChangedEventHandler? PropertyChanged;

   protected virtual void OnPropertyChanged(string propertyName)
   {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }

   protected EntityBase()
   {
      CreatedAt = DateTime.UtcNow;
      ModifiedAt = DateTime.UtcNow;
   }
}