using System.Windows;

using TodoApp.WPF.ViewModels;

namespace TodoApp.WPF.Views;

public partial class SettingsWindow : Window
{
   public SettingsWindow(SettingsViewModel viewModel)
   {
      InitializeComponent();
      DataContext = viewModel;
      Owner = Application.Current.MainWindow;
   }
}