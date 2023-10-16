using IoTServiceAppLibrary.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using SystemTimer = System.Timers.Timer;

namespace IoTServiceAppLibrary.Services;

public class IoTHubManager
{
    private readonly RegistryManager _registryManager;
    private readonly ServiceClient? _serviceClient;
    private readonly string _connectionString = "HostName=kyh-iothub-gm.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=29hqWHz6b0gRxP/Oyo5q7rTahDG6r6sssAIoTDmJCmg=";
    private SystemTimer? _timer;
    public List<DeviceInfo> DeviceList { get; private set; }
    public event Action? DeviceListUpdated;
    public IoTHubManager()
    {
        _registryManager = RegistryManager.CreateFromConnectionString(_connectionString);
        _serviceClient = ServiceClient.CreateFromConnectionString(_connectionString);
        DeviceList = new List<DeviceInfo>();

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

            if (query.HasMoreResults)
                foreach (var twin in await query.GetNextAsTwinAsync())
                    twinList.Add(twin);

            foreach (var device in twinList)
            {
                var deviceToAdd = new DeviceInfo();

                if (!DeviceList.Any(x => x.Id == device.DeviceId))
                    deviceToAdd.Id = device.DeviceId;

                if (device.Properties.Reported.Count > 0)
                {
                    deviceToAdd.Name = device.Properties.Reported["deviceName"].ToString();
                    deviceToAdd.Location = device.Properties.Reported["location"].ToString();
                    deviceToAdd.Type = device.Properties.Reported["deviceType"].ToString();
                    deviceToAdd.State = device.Properties.Reported["state"].ToString();
                    deviceToAdd.ConnectionState = device.ConnectionState.ToString()!;
                }
                DeviceList.Add(deviceToAdd);
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
    public async Task InvokeDirectMethodOnCloudAsync(string deviceId, string methodName)
    {
        try
        {
            var method = new CloudToDeviceMethod(methodName);
            await _serviceClient!.InvokeDeviceMethodAsync(deviceId, method);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}
