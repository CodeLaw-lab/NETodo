using System.Windows;
using TodoApp.ViewModels;

namespace TodoApp.WPF;

public partial class MainWindow : Window
{
   public MainWindow(MainViewModel viewModel)
   {
      InitializeComponent();
      DataContext = viewModel;
   }
}