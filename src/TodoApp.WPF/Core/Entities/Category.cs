using System.Collections.ObjectModel;

namespace TodoApp.WPF.Core.Entities;

public class Category : EntityBase
{
   private string _name = string.Empty;
   private string _color = "#007ACC";
   private string _icon = "FolderOutline";
   private ObservableCollection<TodoTask> _tasks = new();

   public string Name
   {
      get => _name;
      set => SetField(ref _name, value, nameof(Name));
   }

   public string Color
   {
      get => _color;
      set => SetField(ref _color, value, nameof(Color));
   }

   public string Icon
   {
      get => _icon;
      set => SetField(ref _icon, value, nameof(Icon));
   }

   public ObservableCollection<TodoTask> Tasks
   {
      get => _tasks;
      set => SetField(ref _tasks, value, nameof(Tasks));
   }

   public bool CanDelete()
   {
      return Tasks.Count == 0;
   }

   public void Update(string name, string color, string icon)
   {
      Name = name;
      Color = color;
      Icon = icon;
      ModifiedAt = DateTime.UtcNow;
   }
}