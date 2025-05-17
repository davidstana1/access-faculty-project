namespace backend.dto.acess_log;

public class CreateAccessLogDto
{
    public int EmployeeId { get; set; }
    public string Direction { get; set; }
    public string Method { get; set; }
    public string VehicleNumber { get; set; }
    public bool Override { get; set; }
    public bool WasOverridden { get; set; }
    public string OverrideUserId { get; set; }
}