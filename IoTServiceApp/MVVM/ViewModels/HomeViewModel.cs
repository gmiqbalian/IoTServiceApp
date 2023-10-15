using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IoTServiceAppLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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

    [ObservableProperty]
    ObservableCollection<DeviceInfoViewModel> devices;

    ObservableCollection<string> test = new ObservableCollection<string>();

    public HomeViewModel(IServiceProvider serviceProvider, 
        IoTHubManager iotHubManager, 
        DateAndTimeService dateAndTimeService, 
        WeatherService weatherService)
    {
        _serviceProvider = serviceProvider;
        _iotHubManager = iotHubManager;
        _dateAndTimeService = dateAndTimeService;
        _weatherService = weatherService;

        devices = new ObservableCollection<DeviceInfoViewModel>();
        test.Add("hello");
        test.Add("hello1");
        test.Add("hello2");


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
    private void SwitchDeviceOnOff()
    {
        DeviceState = "i am triggered";
    }
}
