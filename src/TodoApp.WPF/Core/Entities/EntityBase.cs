using System.ComponentModel;

namespace TodoApp.WPF.Core.Entities;

public abstract class EntityBase : INotifyPropertyChanged
{
   private int _id;
   private DateTime _createdAt = DateTime.UtcNow;
   private DateTime _modifiedAt = DateTime.UtcNow;
   private bool _isDeleted;

   public int Id
   {
      get => _id;
      set => SetField(ref _id, value, nameof(Id));
   }

   public DateTime CreatedAt
   {
      get => _createdAt;
      set => SetField(ref _createdAt, value, nameof(CreatedAt));
   }

   public DateTime ModifiedAt
   {
      get => _modifiedAt;
      set => SetField(ref _modifiedAt, value, nameof(ModifiedAt));
   }

   public bool IsDeleted
   {
      get => _isDeleted;
      set => SetField(ref _isDeleted, value, nameof(IsDeleted));
   }

   public event PropertyChangedEventHandler? PropertyChanged;

   protected virtual void OnPropertyChanged(string propertyName)
   {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }

   protected bool SetField<T>(ref T field, T value, string propertyName)
   {
      if (EqualityComparer<T>.Default.Equals(field, value)) return false;
      field = value;
      OnPropertyChanged(propertyName);
      return true;
   }
}