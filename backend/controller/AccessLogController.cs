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
    [Route("api/[controller]")]
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
                    Direction = log.Direction.ToString(),
                    Method = log.Method.ToString(),
                    VehicleNumber = log.VehicleNumber,
                    IsWithinSchedule = log.IsWithinSchedule,
                    WasOverridden = log.WasOverridden,
                    OverrideUserId = log.OverrideUserId
                })
                .ToListAsync();

            return accessLogs;
        }

        // POST: api/AccessLog
        //THIS WILL BE CHANGED, NEED TO FIND THE LOGIC TO GET THE LOGS IN REAL TIME
        [HttpPost]
        [Authorize(Roles = "GatePersonnel,HR")]
        public async Task<ActionResult<AccessLogDto>> CreateAccessLog(CreateAccessLogDto logDto)
        {
            var employee = await _context.Employees.FindAsync(logDto.EmployeeId);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            // Check if the employee has access enabled
            if (!employee.IsAccessEnabled)
            {
                // If GatePersonnel is creating the log, they can override
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (logDto.Override)
                {
                    logDto.WasOverridden = true;
                    logDto.OverrideUserId = userId;
                }
                else
                {
                    return BadRequest("Employee access is disabled");
                }
            }

            // Check if within schedule - in a real system, would check against AccessSchedule
            bool isWithinSchedule = IsWithinSchedule(logDto.EmployeeId);

            var accessLog = new AccessLog
            {
                EmployeeId = logDto.EmployeeId,
                Timestamp = DateTime.UtcNow,
                Direction = (AccessDirection)Enum.Parse(typeof(AccessDirection), logDto.Direction),
                Method = (AccessMethod)Enum.Parse(typeof(AccessMethod), logDto.Method),
                VehicleNumber = logDto.VehicleNumber,
                IsWithinSchedule = isWithinSchedule,
                WasOverridden = logDto.WasOverridden,
                OverrideUserId = logDto.OverrideUserId
            };

            _context.AccessLogs.Add(accessLog);
            await _context.SaveChangesAsync();

            var result = new AccessLogDto
            {
                Id = accessLog.Id,
                EmployeeId = accessLog.EmployeeId,
                EmployeeName = employee.FirstName + " " + employee.LastName,
                Timestamp = accessLog.Timestamp,
                Direction = accessLog.Direction.ToString(),
                Method = accessLog.Method.ToString(),
                VehicleNumber = accessLog.VehicleNumber,
                IsWithinSchedule = accessLog.IsWithinSchedule,
                WasOverridden = accessLog.WasOverridden,
                OverrideUserId = accessLog.OverrideUserId
            };

            return CreatedAtAction(nameof(GetEmployeeAccessLogs), new { employeeId = result.EmployeeId }, result);
        }

        // Helper method to check if an access is within schedule
        // In a real implementation, this would check against the AccessSchedule records
        private bool IsWithinSchedule(int employeeId)
        {
            // Get current day and time
            var now = DateTime.UtcNow;
            var dayOfWeek = now.DayOfWeek;
            var timeOfDay = now.TimeOfDay;

            // For demo purposes, let's just check if it's a weekday between 8 AM and 6 PM
            if (dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday)
            {
                if (timeOfDay >= TimeSpan.FromHours(8) && timeOfDay <= TimeSpan.FromHours(18))
                {
                    return true;
                }
            }

            return false;
        }
    }
}