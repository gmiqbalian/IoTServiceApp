using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IoTServiceApp.Enums;
using IoTServiceApp.MVVM.Controls;
using IoTServiceApp.MVVM.Views;
using IoTServiceAppLibrary.Services;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace IoTServiceApp.MVVM.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IoTHubManager _iotHubManager;
    private readonly HttpClient _httpClient;
    private DispatcherTimer _timer;

    [ObservableProperty]
    private ObservableCollection<DeviceInfoViewModel> _devices;

    [ObservableProperty]
    private Control _currentSection;

    [ObservableProperty]
    private string? _deviceIdUserInput;

    [ObservableProperty]
    private string? _iotHubConnectionStringUserinput;

    [ObservableProperty]
    private string? _outsideTempUrlUserInput;

    [ObservableProperty]
    private int _outsideTempUpdateFrequencyUserInput;

    [ObservableProperty]
    private string? _statusMessage;

    public SettingsViewModel(IServiceProvider serviceProvider, IoTHubManager iotHubManager, HttpClient httpClient)
    {
        _serviceProvider = serviceProvider;
        _iotHubManager = iotHubManager;
        _httpClient = httpClient;
        _devices = new ObservableCollection<DeviceInfoViewModel>();

        CurrentSection = _serviceProvider.GetRequiredService<DeviceListControl>();

        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(3);
        _timer.Tick += (s, e) => Timer_Tick();
    }

    [RelayCommand]
    private void NavigateToHome()
    {
        var mainWindowViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
        mainWindowViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<HomeViewModel>();
        CurrentSection = _serviceProvider.GetRequiredService<DeviceListControl>();
    }
    [RelayCommand]
    private static void ExitApp()
    {
        Environment.Exit(0);
    }
    [RelayCommand]
    private void NavigateToAddDeviceSection()
    {
        CurrentSection = _serviceProvider.GetRequiredService<AddDeviceControl>();
    }
    [RelayCommand]
    private void NavigateToDeviceListSection()
    {
        CurrentSection = _serviceProvider.GetRequiredService<DeviceListControl>();
    }
    [RelayCommand]
    private async Task AddDeviceToCloud()
    {
        var deviceId = DeviceIdUserInput;

        if (!string.IsNullOrEmpty(deviceId))
        {
            try
            {
                var device = await _iotHubManager.GetDeviceFromCloudAsync(deviceId);
                if (device is not null)
                {
                    ShowMessage(MessageStatus.Duplicate);
                    return;
                }

                await _iotHubManager.RegisterDeviceToCloudAsync(deviceId);
                DeviceIdUserInput = null;
                ShowMessage(MessageStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ShowMessage(MessageStatus.Error);
            }
        }
        else
            ShowMessage(MessageStatus.Empty);
    }
    private void ShowMessage(MessageStatus status)
    {
        StatusMessage = string.Empty;

        switch (status)
        {
            case MessageStatus.Success:
                StatusMessage = "Executed successfully!";
                break;

            case MessageStatus.Error:
                StatusMessage = "Some error occurred.";
                break;

            case MessageStatus.Empty:
                StatusMessage = "Some field is/are empty";
                break;

            case MessageStatus.Duplicate:
                StatusMessage = "Device Id already exists";
                break;
        }

        _timer.Start();
    }
    private void Timer_Tick()
    {
        StatusMessage = string.Empty;
        _timer.Stop();
    }

    [RelayCommand]
    private async Task DeleteDeviceFromCloud(string deviceId)
    {
        if (!string.IsNullOrEmpty(deviceId))
            await _iotHubManager.DeleteDeviceFromCloudAsync(deviceId);
    }

    [RelayCommand]
    private async Task UpdateAppSettings()
    {
        if (IotHubConnectionStringUserinput is null &&
            OutsideTempUrlUserInput is null &&
            OutsideTempUpdateFrequencyUserInput == 0)
        {
            ShowMessage(MessageStatus.Empty);
            return;
        }


        var result = await _iotHubManager.UpdateSettingsInDataBase(
            IotHubConnectionStringUserinput!,
            OutsideTempUrlUserInput!,
            OutsideTempUpdateFrequencyUserInput!);

        if (result)
            ShowMessage(MessageStatus.Success);
        else ShowMessage(MessageStatus.Error);
    }
    [RelayCommand]
    private void NavigateToAppSettingsSection()
    {
        CurrentSection = _serviceProvider
            .GetRequiredService<SettingsSectionControl>();
    }
}
