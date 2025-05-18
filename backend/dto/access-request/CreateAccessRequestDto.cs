namespace backend.dto.access_request;

public class CreateAccessRequestDto
{
    public int EmployeeId { get; set; }
    public string Direction { get; set; }
    public string Method { get; set; }
    public string VehicleNumber { get; set; }
}