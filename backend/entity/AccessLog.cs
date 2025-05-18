using backend.enums;

namespace backend.entity;

public class AccessLog
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public DateTime Timestamp { get; set; }
    public AccessDirection Direction { get; set; }
    public AccessMethod Method { get; set; }
    public string VehicleNumber { get; set; }
}