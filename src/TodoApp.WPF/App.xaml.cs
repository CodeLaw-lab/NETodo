using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Windows;
using TodoApp.WPF.Core.Interfaces;
using TodoApp.WPF.Infrastructure.Data;
using TodoApp.WPF.Infrastructure.Repositories;
using TodoApp.WPF.Services;
using TodoApp.WPF.ViewModels;
using TodoApp.WPF.Views;
using MainViewModel = TodoApp.WPF.ViewModels.MainViewModel;

namespace TodoApp.WPF;

public partial class App : Application
{
   private readonly IHost _host;

   public App()
   {
      _host = Host.CreateDefaultBuilder()
         .ConfigureAppConfiguration((context, config) =>
         {
            config.SetBasePath(Directory.GetCurrentDirectory());
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
         })
         .ConfigureServices((context, services) =>
         {
            ConfigureServices(services, context.Configuration);
         })
         .Build();
   }

   private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
   {
      // Регистрация DbContext
      var connectionString = configuration.GetConnectionString("DefaultConnection")
                             ?? "Data Source=todoapp.db";

      services.AddDbContext<AppDbContext>(options =>
         options.UseSqlite(connectionString));

      // Регистрация репозиториев
      services.AddScoped<ITaskRepository, TaskRepository>();
      services.AddScoped<ICategoryRepository, CategoryRepository>();

      // Регистрация сервисов
      services.AddScoped<ITaskService, TaskService>();
      services.AddScoped<ICategoryService, CategoryService>();
      services.AddSingleton<IDialogService, DialogService>();
      services.AddSingleton<INavigationService, NavigationService>();

      // Регистрация ViewModels
      services.AddSingleton<MainViewModel>();
      services.AddTransient<TaskEditViewModel>();

      // Регистрация MainWindow
      services.AddSingleton<MainWindow>();
      services.AddTransient<TaskEditWindow>();
   }

   protected override async void OnStartup(StartupEventArgs e)
   {
      await _host.StartAsync();

      // Создание базы данных при запуске
      using (var scope = _host.Services.CreateScope())
      {
         var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
         await dbContext.Database.EnsureCreatedAsync();

         // Создание тестовых данных для демонстрации
         await InitializeSampleDataAsync(scope.ServiceProvider);
      }

      var mainWindow = _host.Services.GetRequiredService<MainWindow>();
      mainWindow.Show();

      base.OnStartup(e);
   }

   private async Task InitializeSampleDataAsync(IServiceProvider serviceProvider)
   {
      var categoryService = serviceProvider.GetRequiredService<ICategoryService>();
      var taskService = serviceProvider.GetRequiredService<ITaskService>();

      // Проверяем, есть ли уже категории
      var categories = await categoryService.GetAllCategoriesAsync();
      if (!categories.Any())
      {
         // Создаем категорию по умолчанию
         var defaultCategory = new Core.Entities.Category
         {
            Name = "Общее",
            Color = "#007ACC",
            Icon = "FolderOutline"
         };
         await categoryService.CreateCategoryAsync(defaultCategory);

         // Создаем несколько тестовых задач
         var tasks = new[]
         {
            new Core.Entities.TodoTask
            {
               Title = "Изучить WPF",
               Description = "Изучить основы WPF и паттерн MVVM",
               Priority = Core.Enums.Priority.High,
               DueDate = DateTime.UtcNow.AddDays(7),
               CategoryId = 1
            },
            new Core.Entities.TodoTask
            {
               Title = "Купить продукты",
               Description = "Молоко, хлеб, яйца",
               Priority = Core.Enums.Priority.Medium,
               DueDate = DateTime.UtcNow.AddDays(1)
            },
            new Core.Entities.TodoTask
            {
               Title = "Закончить проект TodoApp",
               Description = "Завершить разработку приложения TodoApp",
               Priority = Core.Enums.Priority.Critical,
               DueDate = DateTime.UtcNow.AddDays(14),
               CategoryId = 1
            }
         };

         foreach (var task in tasks)
         {
            await taskService.CreateTaskAsync(task);
         }
      }
   }

   protected override async void OnExit(ExitEventArgs e)
   {
      using (_host)
      {
         await _host.StopAsync(TimeSpan.FromSeconds(5));
      }

      base.OnExit(e);
   }
}