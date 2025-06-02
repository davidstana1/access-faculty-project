using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using backend.data;
using backend.dto.access_request;
using backend.enums;
using backend.service.access.access_request;
using Microsoft.EntityFrameworkCore;

namespace backend.controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class EspController : ControllerBase
    {
        private readonly ILogger<EspController> _logger;
        private readonly ApplicationDbContext _context;

        public EspController(ILogger<EspController> logger,ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveData([FromServices] ApplicationDbContext dbContext, [FromServices] IAccessRequestService accessRequestService)
        {
            using var reader = new StreamReader(Request.Body);
            var securityCode = await reader.ReadToEndAsync();

            _logger.LogInformation("Cod primit de la ESP: {Data}", securityCode);

            // 1. Caută angajatul după BluetoothSecurityCode
            var employee = await dbContext.Employees.FirstOrDefaultAsync(e => e.BluetoothSecurityCode == securityCode);

            if (employee == null)
            {
                return NotFound(new { error = "Angajatul nu a fost găsit pentru codul primit" });
            }
            _logger.LogInformation($"Employee + {employee.FirstName} {employee.LastName}");

            // 2. Creează DTO-ul pentru request
            var requestDto = new CreateAccessRequestDto
            {
                EmployeeId = employee.Id,
                Method = AccessMethod.Vehicle.ToString(),
                Direction = AccessDirection.Entry.ToString(),
                VehicleNumber = employee.VehicleNumber
            };

            // 3. Creează cererea de acces
            var accessRequest = await accessRequestService.CreateRequestAsync(requestDto);

            // 4. Returnează cererea creată
            return Ok(new
            {
                status = "Access request created",
                employee = new { employee.FirstName, employee.LastName, employee.VehicleNumber },
                request = accessRequest
            });
        }
        
        [HttpPost("mobile")]
        public async Task<IActionResult> ReceiveDataFromMobile()
        {
            using var reader = new StreamReader(Request.Body);
            var data = await reader.ReadToEndAsync();

            _logger.LogInformation("Date primite de la mobile: {Data}", data);

            // TODO: parsează JSON-ul și extrage PUK-ul dacă e cazul

            return Ok(new { status = "received", content = data });
        }
    
        [HttpPost("send-to-esp")]
        public async Task<IActionResult> SendToEsp([FromBody] string data)
        {
            var espIp = "http://192.168.220.72"; // IP-ul ESP-ului (vezi în Serial Monitor)
            var url = $"{espIp}/data";

            using var client = new HttpClient();
            var content = new StringContent(data, System.Text.Encoding.UTF8, "text/plain");

            try
            {
                var response = await client.PostAsync(url, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                return Ok(new { status = "sent", response = responseBody });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to send data to ESP", details = ex.Message });
            }
        }

    }
}
