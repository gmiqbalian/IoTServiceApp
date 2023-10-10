using IoTServiceApp.MVVM.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using SystemTimer = System.Timers.Timer;

namespace IoTServiceApp.Services;

public class IoTHubManager
{
    private readonly RegistryManager _registryManager;
    private SystemTimer timer;
    public List<DeviceInfo> DeviceList { get; private set; }
    public event Action DeviceListUpdated;
    public IoTHubManager()
    {
        _registryManager = RegistryManager.CreateFromConnectionString("HostName=kyh-iothub-gm.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=29hqWHz6b0gRxP/Oyo5q7rTahDG6r6sssAIoTDmJCmg=");
        
        DeviceList = new List<DeviceInfo>();

        Task.Run(GetAllDevicesFromCloudAsync);

        //timer = new Timer(1000);
        //timer.Elapsed += async (s, e) => await GetAllDevicesFromCloudAsync();
        //timer.Start();

    }
    public async Task GetAllDevicesFromCloudAsync()
    {
        try
        {
            var twinList = new List<Twin>();
            var query = _registryManager.CreateQuery("SELECT * FROM devices");

            foreach (var twin in await query.GetNextAsTwinAsync())
                twinList.Add(twin);

            foreach (var device in twinList)
            {
                if (!DeviceList.Any(x => x.Id == device.DeviceId))
                    DeviceList.Add(new DeviceInfo { Id = device.DeviceId });
            }

            for (int i = DeviceList.Count - 1; i >= 0; i--)
            {
                if (!twinList.Any(x => x.DeviceId == DeviceList[i].Id))
                    DeviceList.RemoveAt(i);
            }

            DeviceListUpdated.Invoke();

        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
    }
    private void UpdateDeviceList()
    {

    }
}
