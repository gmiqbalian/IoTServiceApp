using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using IoTServiceApp.MVVM.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IoTServiceApp
{
    public partial class App : Application
    {
        private static IHost _appHost { get; set; }
        public App()
        {
            _appHost = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<SettingsViewModel>();
                    services.AddSingleton<HomeViewModel>();
                })
                .Build();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            await _appHost.StartAsync();

            var mainWindow = _appHost.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}
