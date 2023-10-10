using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IoTServiceApp.MVVM.Models;
using IoTServiceApp.Services;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace IoTServiceApp.MVVM.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IoTHubManager _iotManager;

    [ObservableProperty]
    ObservableCollection<DeviceInfoViewModel> devices;

    public HomeViewModel(IServiceProvider serviceProvider, IoTHubManager iotHubManager)
    {
        _serviceProvider = serviceProvider;
        _iotManager = iotHubManager;

        GetAllDevices();
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

        //Devices = new ObservableCollection<DeviceInfo>(_iotManager.DeviceList
        //    .Select(device => new DeviceInfo { Id = device.Id }).ToList());

        //Devices = new ObservableCollection<DeviceInfo>()
        //{
        //    new DeviceInfo {Id="machine"},
        //    new DeviceInfo {Id="tv"},
        //    new DeviceInfo {Id="speakers"},
        //};
    }
}
