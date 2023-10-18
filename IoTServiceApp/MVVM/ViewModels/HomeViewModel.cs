using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IoTServiceAppLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace IoTServiceApp.MVVM.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IoTHubManager _iotHubManager;
    private readonly DateAndTimeService _dateAndTimeService;
    private readonly WeatherService _weatherService;

    [ObservableProperty]
    private string? _time;

    [ObservableProperty]
    private string? _date;

    [ObservableProperty]
    private string? _outsideTemp;

    [ObservableProperty]
    private string? _weatherIcon;

    [ObservableProperty]
    private string? _deviceState;

    public string test = "hello this is from home view model";

    [ObservableProperty]
    ObservableCollection<DeviceInfoViewModel>? devices = new ObservableCollection<DeviceInfoViewModel>();

    public HomeViewModel(IServiceProvider serviceProvider,
        IoTHubManager iotHubManager,
        DateAndTimeService dateAndTimeService,
        WeatherService weatherService)
    {
        _serviceProvider = serviceProvider;
        _iotHubManager = iotHubManager;
        _dateAndTimeService = dateAndTimeService;
        _weatherService = weatherService;

        GetAllDevices();
        GetDateAndTime();
        GetWeatherData();

        _iotHubManager.DeviceListUpdated += GetAllDevices;
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        var mainWindowViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
        mainWindowViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<SettingsViewModel>();
    }
    private void GetAllDevices()
    {
        Devices = new ObservableCollection<DeviceInfoViewModel>(_iotHubManager.DeviceList
            .Select(device => new DeviceInfoViewModel(device)).ToList());
    }
    [RelayCommand]
    private async Task SwitchDeviceOnOff(Button button)
    {
        var btn = (DeviceInfoViewModel)button.DataContext;

        if(!string.IsNullOrEmpty(btn.DeviceInfo.State))
            if(btn.DeviceInfo.ConnectionState.ToLower() == "connected")
                if (btn.DeviceInfo.State.ToLower() == "off")
                {
                    await _iotHubManager.InvokeDirectMethodOnCloudAsync(btn.DeviceInfo.Id, "start");
                    DeviceState = "ON";
                    var b = button.Template.FindName("showstate", button);
                }
                else if (btn.DeviceInfo.State.ToLower() == "on")
                {
                    await _iotHubManager.InvokeDirectMethodOnCloudAsync(btn.DeviceInfo.Id, "stop");
                    DeviceState = "OFF";
                }
    }
    private void GetDateAndTime()
    {
        _dateAndTimeService.TimeUpdated += () =>
        {
            Time = _dateAndTimeService.Time;
            Date = _dateAndTimeService.Date;
        };
    }
    private void GetWeatherData()
    {
        _weatherService.WeatheUpdated += () =>
        {
            OutsideTemp = _weatherService.OutsideTemp;
            WeatherIcon = _weatherService.WeatherIcon;
        };
    }
    [RelayCommand]
    private async Task DeleteDeviceFromCloudHome(string deviceId)
    {
        if (!string.IsNullOrEmpty(deviceId))
            await _iotHubManager.DeleteDeviceFromCloudAsync(deviceId);
    }

}
