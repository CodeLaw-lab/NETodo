using TodoApp.WPF.Models;

namespace TodoApp.WPF.Core.Interfaces;

public interface IConfigurationService
{
   AppSettings AppSettings { get; }
   Task SaveAsync();
   Task LoadAsync();
}