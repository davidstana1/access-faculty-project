namespace backend.dto.access_request;

public class AccessRequestDto
{
    public string Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Direction { get; set; } // "Entry" or "Exit"
    public string Method { get; set; } // "Badge", "Bluetooth", etc.
    public string VehicleNumber { get; set; }
}