using backend.enums;

namespace backend.dto.acess_log;

public class CreateAccessLogDto
{
    public int EmployeeId { get; set; }
    public DateTime Timestamp { get; set; }
    public AccessDirection Direction { get; set; }
    public AccessMethod Method { get; set; }
    public string VehicleNumber { get; set; }

}