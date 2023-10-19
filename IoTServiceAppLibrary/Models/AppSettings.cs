using System.ComponentModel.DataAnnotations;

namespace IoTServiceAppLibrary.Models;

public class AppSettings
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? IotHubConnectionString { get; set; }
    public string? WeatherApiUrl { get; set; }
    public double? WeatherUpateInterval { get; set; }

}
