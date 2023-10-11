using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IoTServiceAppLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace IoTServiceApp.MVVM.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IoTHubManager _iotManager;
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
    ObservableCollection<DeviceInfoViewModel> devices;

    public HomeViewModel(IServiceProvider serviceProvider, 
        IoTHubManager iotHubManager, 
        DateAndTimeService dateAndTimeService, 
        WeatherService weatherService)
    {
        _serviceProvider = serviceProvider;
        _iotManager = iotHubManager;
        _dateAndTimeService = dateAndTimeService;
        _weatherService = weatherService;

        devices = new ObservableCollection<DeviceInfoViewModel>();

        GetAllDevices();
        GetDateAndTime();
        GetWeatherData();

        _iotManager.DeviceListUpdated += GetAllDevices;
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        var mainWindowViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
        mainWindowViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<SettingsViewModel>();
    }
    private void GetAllDevices()
    {
        Devices = new ObservableCollection<DeviceInfoViewModel>(_iotManager.DeviceList
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
}
