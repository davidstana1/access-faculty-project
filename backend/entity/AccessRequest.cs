using backend.enums;

namespace backend.entity;

public class AccessRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int EmployeeId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Direction { get; set; }
    public string Method { get; set; }
    public string VehicleNumber { get; set; }
    public AccessRequestStatus Status { get; set; }
}