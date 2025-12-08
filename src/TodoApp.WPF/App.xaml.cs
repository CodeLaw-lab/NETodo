using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoApp.WPF.Core.Interfaces;
using TodoApp.WPF.Infrastructure.Data;
using TodoApp.WPF.Infrastructure.Repositories;

namespace TodoApp.WPF
{
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
         // Регистрация DbContext с использованием строки подключения из конфигурации
         var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=todoapp.db";

         services.AddDbContext<AppDbContext>(options =>
             options.UseSqlite(connectionString));

         // Регистрация репозиториев
         services.AddScoped<ITaskRepository, TaskRepository>();
         services.AddScoped<ICategoryRepository, CategoryRepository>();

         // Регистрация MainWindow
         services.AddSingleton<MainWindow>();
      }

      protected override async void OnStartup(StartupEventArgs e)
      {
         await _host.StartAsync();

         // Создание базы данных при запуске
         using (var scope = _host.Services.CreateScope())
         {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
         }

         var mainWindow = _host.Services.GetRequiredService<MainWindow>();
         mainWindow.Show();

         base.OnStartup(e);
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
}