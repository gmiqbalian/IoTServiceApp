using CommunityToolkit.Mvvm.ComponentModel;
using IoTServiceApp.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
