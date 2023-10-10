using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using IoTServiceApp.MVVM.Controls;
using IoTServiceApp.MVVM.ViewModels;
using IoTServiceApp.MVVM.Views;
using IoTServiceApp.Services;
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
                    services.AddSingleton<DateAndTimeService>();
                    services.AddSingleton<WeatherService>();
                    services.AddSingleton<IoTHubManager>();
                    services.AddSingleton<HttpClient>();

                    services.AddSingleton<MainWindow>();
                    
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<HomeViewModel>();
                    services.AddSingleton<SettingsViewModel>();
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
