using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
      services.AddSingleton<MainWindow>();
   }

   protected override async void OnStartup(StartupEventArgs e)
   {
      await _host.StartAsync();

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