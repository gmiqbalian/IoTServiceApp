namespace IoTServiceApp.MVVM.Models;

public class DeviceInfo
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string Icon => SetIcon();
    private string SetIcon()
    {
        return (Type.ToLower()) switch
        {
            "washingmachine" => "\uf898",
            "speaker" => "\uf8df",
            "tv" => "\uf26c",
            _ => ""
        };
    }
}
