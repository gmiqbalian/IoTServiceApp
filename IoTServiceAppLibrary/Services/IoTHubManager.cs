using IoTServiceAppLibrary.Contexts;
using IoTServiceAppLibrary.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using SystemTimer = System.Timers.Timer;

namespace IoTServiceAppLibrary.Services;

public class IoTHubManager
{
    private RegistryManager? _registryManager;
    private ServiceClient? _serviceClient;
    private string? IotHubConnectionString { get; set; }
    private SystemTimer? _timer;
    private readonly DataContext _dbcontext;
    private readonly IConfiguration _configuration;
    public List<DeviceInfo> DeviceList { get; private set; }
    public event Action? DeviceListUpdated;
    public IoTHubManager(DataContext dbcontext, IConfiguration configuration)
    {
        _configuration = configuration;
        _dbcontext = dbcontext;
        DeviceList = new List<DeviceInfo>();

        Task.WhenAll(LoadSettingsFromDatabase(),
            GetAllDevicesFromCloudAsync());

        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += async (s, e) => await GetAllDevicesFromCloudAsync();
        _timer.Start();
    }
    public async Task LoadSettingsFromDatabase()
    {
        var appSettings = await _dbcontext.AppSettings.FirstOrDefaultAsync();
        
        if (appSettings is not null)
            IotHubConnectionString = appSettings.IotHubConnectionString!;
        else
        {
            appSettings ??= new AppSettings();
            appSettings.IotHubConnectionString = _configuration
                .GetConnectionString("iotHubConnectionString");
            appSettings.WeatherUpateInterval = 60000 * 5;
            appSettings.WeatherApiUrl = _configuration.GetConnectionString("weatherUrl");

            await _dbcontext.AppSettings.AddAsync(appSettings);
            await _dbcontext.SaveChangesAsync();
            
            IotHubConnectionString = appSettings.IotHubConnectionString;
        }
        
        _registryManager = RegistryManager.CreateFromConnectionString(IotHubConnectionString);
        _serviceClient = ServiceClient.CreateFromConnectionString(IotHubConnectionString);
    }
    public async Task<bool> UpdateSettingsInDataBase(string connectionString, string tempUrl, int tempLoadTime)
    {
        var appSettings = await _dbcontext.AppSettings.FirstOrDefaultAsync();
        if (appSettings is not null)
        {
            appSettings.IotHubConnectionString = connectionString ?? appSettings.IotHubConnectionString;
            appSettings.WeatherApiUrl = tempUrl ?? appSettings.WeatherApiUrl;

            if (tempLoadTime > 0)
                appSettings.WeatherUpateInterval = tempLoadTime;

            _dbcontext.AppSettings.Update(appSettings);
            _dbcontext.SaveChanges();
            return true;
        }
        return false;
    }
    public async Task GetAllDevicesFromCloudAsync()
    {
        try
        {
            var twinList = new List<Twin>();
            var query = _registryManager!.CreateQuery("SELECT * FROM devices");

            if (query.HasMoreResults)
                foreach (var twin in await query.GetNextAsTwinAsync())
                    twinList.Add(twin);

            foreach (var twin in twinList)
            {
                if (!DeviceList.Any(x => x.Id == twin.DeviceId))
                {
                    var deviceToAdd = new DeviceInfo();
                    deviceToAdd.Id = twin.DeviceId;

                    DeviceList.Add(deviceToAdd);
                }

                var device = DeviceList.FirstOrDefault(x => x.Id == twin.DeviceId);

                device!.ConnectionState = twin.ConnectionState.ToString()!;
                device!.Name = twin.Properties.Reported.Contains("deviceName") ? twin.Properties.Reported["deviceName"] : "";
                device!.Location = twin.Properties.Reported.Contains("location") ? twin.Properties.Reported["location"] : "";
                device!.Type = twin.Properties.Reported.Contains("deviceType") ? twin.Properties.Reported["deviceType"] : "";
                device!.State = twin.Properties.Reported.Contains("state") ? twin.Properties.Reported["state"] : "";
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
    public async Task DeleteDeviceFromCloudAsync(string deviceId)
    {
        try
        {
            await _registryManager.RemoveDeviceAsync(deviceId);
        } 
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
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
