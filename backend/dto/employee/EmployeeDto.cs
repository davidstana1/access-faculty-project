namespace backend.dto.employee;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CNP { get; set; }
    public string BadgeNumber { get; set; }
    public string PhotoUrl { get; set; }
    public int DivisionId { get; set; }
    public string DivisionName { get; set; }
    public string BluetoothSecurityCode { get; set; }
    public string VehicleNumber { get; set; }
    public bool IsAccessEnabled { get; set; }
    public DateTime ApprovalDate { get; set; }
}