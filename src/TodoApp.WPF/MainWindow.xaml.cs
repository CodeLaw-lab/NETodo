using System.Windows;
using TodoApp.WPF.ViewModels;

namespace TodoApp.WPF;

public partial class MainWindow : Window
{
   public MainWindow(MainViewModel viewModel)
   {
      InitializeComponent();
      DataContext = viewModel;
   }
}