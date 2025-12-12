using System.Windows;
using MainViewModel = TodoApp.WPF.ViewModels.MainViewModel;

namespace TodoApp.WPF;

public partial class MainWindow : Window
{
   public MainWindow(MainViewModel viewModel)
   {
      InitializeComponent();
      DataContext = viewModel;
   }
}