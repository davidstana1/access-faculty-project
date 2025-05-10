using backend.enums;

namespace backend.entity;

public class GateStatus
{
    public int Id { get; set; }
    public GateState State { get; set; }
    public DateTime LastUpdated { get; set; }
    public int? LastAccessEmployeeId { get; set; }
    public Employee LastAccessEmployee { get; set; }
    public AccessDirection? LastDirection { get; set; }
}