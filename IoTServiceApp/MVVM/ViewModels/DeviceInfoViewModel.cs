using CommunityToolkit.Mvvm.ComponentModel;
using IoTServiceAppLibrary.Models;

namespace IoTServiceApp.MVVM.ViewModels;

public partial class DeviceInfoViewModel : ObservableObject
{
    [ObservableProperty]
    private DeviceInfo _deviceInfo;

    public DeviceInfoViewModel(DeviceInfo deviceInfo)
    {
        _deviceInfo = deviceInfo;
    }
}
