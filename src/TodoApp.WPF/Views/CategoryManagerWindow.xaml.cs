using System.Windows;

using TodoApp.WPF.ViewModels;

namespace TodoApp.WPF.Views;

public partial class CategoryManagerWindow : Window
{
   public CategoryManagerWindow(CategoryManagerViewModel viewModel)
   {
      InitializeComponent();
      DataContext = viewModel;
      Owner = Application.Current.MainWindow;
   }
}