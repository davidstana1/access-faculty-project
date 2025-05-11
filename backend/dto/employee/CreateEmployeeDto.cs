using System.ComponentModel.DataAnnotations;

namespace backend.dto.employee;

public class CreateEmployeeDto
{
    [Required]
    public string FirstName { get; set; }
        
    [Required]
    public string LastName { get; set; }
        
    [Required]
    public string CNP { get; set; }
        
    [Required]
    public string BadgeNumber { get; set; }
        
    public string PhotoUrl { get; set; }
        
    [Required]
    public int DivisionId { get; set; }
        
    public string BluetoothSecurityCode { get; set; }
        
    public string VehicleNumber { get; set; }
}