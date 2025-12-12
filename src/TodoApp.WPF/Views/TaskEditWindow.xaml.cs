using System.Windows;

using TodoApp.WPF.ViewModels;

namespace TodoApp.WPF.Views;

public partial class TaskEditWindow : Window
{
   public TaskEditWindow(TaskEditViewModel viewModel)
   {
      InitializeComponent();
      DataContext = viewModel;
      Owner = Application.Current.MainWindow;
   }
}