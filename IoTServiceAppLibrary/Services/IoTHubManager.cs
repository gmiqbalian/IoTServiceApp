using IoTServiceAppLibrary.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using System.Diagnostics;
using SystemTimer = System.Timers.Timer;

namespace IoTServiceAppLibrary.Services;

public class IoTHubManager
{
    private readonly RegistryManager _registryManager;
    private SystemTimer? _timer;
    public List<DeviceInfo> DeviceList { get; private set; } = new List<DeviceInfo>();
    public event Action? DeviceListUpdated;
    public IoTHubManager(string connectionString)
    {
        _registryManager = RegistryManager.CreateFromConnectionString(connectionString);
    }
    public IoTHubManager()
    {
        _registryManager = RegistryManager.CreateFromConnectionString("HostName=kyh-iothub-gm.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=29hqWHz6b0gRxP/Oyo5q7rTahDG6r6sssAIoTDmJCmg=");

        //DeviceList = new List<DeviceInfo>();

        Task.Run(GetAllDevicesFromCloudAsync);

        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += async (s, e) => await GetAllDevicesFromCloudAsync();
        _timer.Start();

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

            DeviceListUpdated?.Invoke();

        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
    }
    public async Task<Device> GetDeviceFromCloudAsync(string deviceId)
    {
        try
        {
            var device = await _registryManager.GetDeviceAsync(deviceId);
            if (device is not null)
                return device;
        }
        catch (Exception e) { Debug.WriteLine(e.Message); }

        return null!;
    }
    public async Task<Device> RegisterDeviceToCloudAsync(string deviceId)
    {
        try
        {
            var deviceToAdd = new Device(deviceId);
            var device = await _registryManager.AddDeviceAsync(deviceToAdd);
            if (device is not null)
                return device;
        }
        catch (Exception e) { Debug.WriteLine(e.Message); }

        return null!;
    }
}
