using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using backend.data;
using backend.dto;
using backend.dto.acess_log;
using backend.entity;
using backend.enums;

namespace backend.controller
{
    [Route("api/AccessLog")]
    [ApiController]
    [Authorize]
    public class AccessLogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AccessLogController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccessLogDto>>> GetAccessLogs(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var accessLogs = await _context.AccessLogs
                .Where(log => log.Timestamp >= startDate && log.Timestamp <= endDate)
                .OrderByDescending(log => log.Timestamp)
                .Select(log => new AccessLogDto
                {
                    Id = log.Id,
                    EmployeeId = log.EmployeeId,
                    EmployeeName = log.Employee.FirstName + " " + log.Employee.LastName,
                    Timestamp = log.Timestamp,
                    Direction = log.Direction,
                    Method = log.Method,
                    VehicleNumber = log.VehicleNumber
                })
                .ToListAsync();

            return accessLogs;
        }

        // GET: api/AccessLog/employee/{employeeId}
        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<AccessLogDto>>> GetEmployeeAccessLogs(int employeeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(userId);
            
            // Get the employee to check permissions
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            // Check if user is in HR role, GatePersonnel, or is a Manager of the employee's division
            bool canAccess = await _userManager.IsInRoleAsync(currentUser, "HR") ||
                             await _userManager.IsInRoleAsync(currentUser, "GatePersonnel") ||
                            (await _userManager.IsInRoleAsync(currentUser, "Manager") && 
                             currentUser.DivisionId.HasValue && 
                             currentUser.DivisionId.Value == employee.DivisionId);
            
            if (!canAccess)
            {
                return Forbid();
            }

            var accessLogs = await _context.AccessLogs
                .Where(log => log.EmployeeId == employeeId)
                .OrderByDescending(log => log.Timestamp)
                .Select(log => new AccessLogDto
                {
                    Id = log.Id,
                    EmployeeId = log.EmployeeId,
                    EmployeeName = log.Employee.FirstName + " " + log.Employee.LastName,
                    Timestamp = log.Timestamp,
                    Direction = log.Direction,
                    Method = log.Method,
                    VehicleNumber = log.VehicleNumber
                })
                .ToListAsync();

            return accessLogs;
        }
        [HttpPost]
        public async Task<ActionResult<AccessLogDto>> CreateAccessLog([FromBody] CreateAccessLogDto logDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = await _context.Employees.FindAsync(logDto.EmployeeId);
            if (employee == null)
            {
                return NotFound($"Employee with ID {logDto.EmployeeId} not found");
            }

            // Create new access log
            var accessLog = new AccessLog
            {
                EmployeeId = logDto.EmployeeId,
                Timestamp = logDto.Timestamp,
                Direction = logDto.Direction,
                Method = logDto.Method,
                VehicleNumber = logDto.VehicleNumber
            };

            _context.AccessLogs.Add(accessLog);
            await _context.SaveChangesAsync();
            
            var accessLogDto = new AccessLogDto
            {
                Id = accessLog.Id,
                EmployeeId = accessLog.EmployeeId,
                EmployeeName = employee.FirstName + " " + employee.LastName,
                Timestamp = accessLog.Timestamp,
                Direction = accessLog.Direction,
                Method = accessLog.Method,
                VehicleNumber = accessLog.VehicleNumber
            };

            return CreatedAtAction(nameof(GetAccessLogs), new { id = accessLog.Id }, accessLogDto);
        }
    }
}
