using backend.enums;

namespace backend.entity;

public class GateStatus
{
    public int Id { get; set; }
    public string State { get; set; }
    public bool IsOperational { get; set; }
    public string LastOperation { get; set; }
    public System.DateTime LastOperationTime { get; set; }
}