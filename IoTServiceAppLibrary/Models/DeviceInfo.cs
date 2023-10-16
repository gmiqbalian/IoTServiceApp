using CommunityToolkit.Mvvm.ComponentModel;

namespace IoTServiceAppLibrary.Models;

public partial class DeviceInfo
{
    public string Id { get; set; } = null!;
    public string? Name { get; set; } = string.Empty;
    public string? Type { get; set; } = string.Empty;
    public string? Location { get; set; } = string.Empty;
    public string? Icon => SetIcon();
    public string State { get; set; } = string.Empty;
    public string ConnectionState { get; set; } = string.Empty;

    private string SetIcon()
    {
        return (Id!.ToLower()) switch
        {
            "washingmachine" => "\uf898",
            "speakers" => "\uf8df",
            "television" => "\uf26c",
            "tv" => "\uf26c",
            _ => "\uf2db"
        };
      
    }
}
