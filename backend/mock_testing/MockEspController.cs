using Microsoft.AspNetCore.Mvc;
using backend.enums;
using backend.entity;
using backend.mock;
using System.Threading.Tasks;

namespace backend.controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class MockController : ControllerBase
    {
        private readonly MockEspGateDevice _mockDevice;
        
        public MockController(MockEspGateDevice mockDevice)
        {
            _mockDevice = mockDevice;
        }
        
        [HttpPost("generate-access-request")]
        public async Task<IActionResult> GenerateAccessRequest([FromBody] MockAccessRequest request)
        {
            // Manually generate an access request for testing
            var httpClient = new HttpClient();
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    EmployeeId = request.EmployeeId,
                    Direction = request.Direction,
                    Method = request.Method,
                    VehicleNumber = request.VehicleNumber
                }),
                System.Text.Encoding.UTF8,
                "application/json");
                
            var response = await httpClient.PostAsync("http://localhost:5203/api/accessRequests", content);
            
            if (response.IsSuccessStatusCode)
            {
                return Ok(new { message = "Mock access request created successfully" });
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { error = errorContent });
            }
        }
    }
    
    public class MockAccessRequest
    {
        public int EmployeeId { get; set; }
        public string Direction { get; set; }
        public string Method { get; set; }
        public string VehicleNumber { get; set; }
    }
}