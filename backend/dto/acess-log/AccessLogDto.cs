namespace backend.dto.acess_log;

public class AccessLogDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public DateTime Timestamp { get; set; }
    public string Direction { get; set; }
    public string Method { get; set; }
    public string VehicleNumber { get; set; }
    public bool IsWithinSchedule { get; set; }
    public bool WasOverridden { get; set; }
    public string OverrideUserId { get; set; }
}
