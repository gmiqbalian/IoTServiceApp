using System.Net.Http;
using System.Windows;
using IoTServiceApp.MVVM.Controls;
using IoTServiceApp.MVVM.ViewModels;
using IoTServiceAppLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IoTServiceApp
{
    public partial class App : Application
    {
        private static IHost? _appHost { get; set; }
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

                    services.AddSingleton<AddDeviceControl>();
                    services.AddSingleton<DeviceListControl>();

                })
                .Build();
        }
        protected override async void OnStartup(StartupEventArgs e)
        {
            await _appHost!.StartAsync();

            var mainWindow = _appHost.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}
