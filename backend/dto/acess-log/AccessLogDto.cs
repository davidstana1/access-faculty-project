using backend.enums;

namespace backend.dto.acess_log;

public class AccessLogDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public DateTime Timestamp { get; set; }
    public AccessDirection Direction { get; set; }
    public AccessMethod Method { get; set; }
    public string VehicleNumber { get; set; }
}
