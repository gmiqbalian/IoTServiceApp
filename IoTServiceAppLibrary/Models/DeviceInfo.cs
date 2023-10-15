namespace IoTServiceAppLibrary.Models;

public class DeviceInfo
{
    public string Id { get; set; } = null!;
    public string? Name { get; set; } = "washingmachine";
    public string? Type { get; set; }
    public string? Location { get; set; }
    public string? Icon => SetIcon();
    public bool State => SetState();
    public string ConnectionState { get; set; }

    public DeviceInfo()
    {
    }
    private string SetIcon()
    {
        return (Name!.ToLower()) switch
        {
            "washingmachine" => "\uf898",
            "speakers" => "\uf8df",
            "television" => "\uf26c",
            _ => ""
        };
      
    }
    private bool SetState()
    {
        if (ConnectionState.ToLower() == "connected")
            return true;
        return false;

    }
}
