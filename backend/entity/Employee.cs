namespace backend.entity;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CNP { get; set; }
    public string BadgeNumber { get; set; } // Număr legitimație
    public string PhotoUrl { get; set; }
    public int DivisionId { get; set; }
    public Division Division { get; set; }
    public string BluetoothSecurityCode { get; set; }
    public string VehicleNumber { get; set; }
    public bool IsAccessEnabled { get; set; } = true;
    public string ApprovedById { get; set; } // ID-ul utilizatorului care a acordat accesul
    public User ApprovedBy { get; set; }
    public DateTime ApprovalDate { get; set; }
    public ICollection<AccessSchedule> AccessSchedules { get; set; }
    public ICollection<AccessLog> AccessLogs { get; set; }
}