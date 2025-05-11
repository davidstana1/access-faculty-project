using System.ComponentModel.DataAnnotations;

namespace backend.dto;

public class UpdateEmployeeDto
{
    public int Id { get; set; }
        
    [Required]
    public string FirstName { get; set; }
        
    [Required]
    public string LastName { get; set; }
        
    public string CNP { get; set; }
        
    public string BadgeNumber { get; set; }
        
    public string PhotoUrl { get; set; }
        
    public int DivisionId { get; set; }
        
    public string BluetoothSecurityCode { get; set; }
        
    public string VehicleNumber { get; set; }
        
    public bool IsAccessEnabled { get; set; }
}