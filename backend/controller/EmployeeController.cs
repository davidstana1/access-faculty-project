using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using backend.data;
using backend.dto;
using backend.dto.employee;
using backend.entity;

namespace backend.controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public EmployeeController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Employee
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(userId);
            
            // Check if user is in HR role - they can see all employees
            if (await _userManager.IsInRoleAsync(currentUser, "HR"))
            {
                var allEmployees = await _context.Employees
                    .Include(e => e.Division)
                    .Select(e => new EmployeeDto
                    {
                        Id = e.Id,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        CNP = e.CNP,
                        BadgeNumber = e.BadgeNumber,
                        PhotoUrl = e.PhotoUrl,
                        DivisionId = e.DivisionId,
                        DivisionName = e.Division.Name,
                        BluetoothSecurityCode = e.BluetoothSecurityCode,
                        VehicleNumber = e.VehicleNumber,
                        IsAccessEnabled = e.IsAccessEnabled,
                        ApprovalDate = e.ApprovalDate
                    })
                    .ToListAsync();
                
                return allEmployees;
            }
            
            // Check if user is a Manager - they can only see employees in their division
            if (await _userManager.IsInRoleAsync(currentUser, "Manager") && currentUser.DivisionId.HasValue)
            {
                var divisionEmployees = await _context.Employees
                    .Include(e => e.Division)
                    .Where(e => e.DivisionId == currentUser.DivisionId.Value)
                    .Select(e => new EmployeeDto
                    {
                        Id = e.Id,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        CNP = e.CNP,
                        BadgeNumber = e.BadgeNumber,
                        PhotoUrl = e.PhotoUrl,
                        DivisionId = e.DivisionId,
                        DivisionName = e.Division.Name,
                        BluetoothSecurityCode = e.BluetoothSecurityCode,
                        VehicleNumber = e.VehicleNumber,
                        IsAccessEnabled = e.IsAccessEnabled,
                        ApprovalDate = e.ApprovalDate
                    })
                    .ToListAsync();
                
                return divisionEmployees;
            }

            // For GatePersonnel or other roles - return only basic employee info needed for access verification
            if (await _userManager.IsInRoleAsync(currentUser, "GatePersonnel"))
            {
                var accessEmployees = await _context.Employees
                    .Where(e => e.IsAccessEnabled)
                    .Select(e => new EmployeeDto
                    {
                        Id = e.Id,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        BadgeNumber = e.BadgeNumber,
                        PhotoUrl = e.PhotoUrl,
                        BluetoothSecurityCode = e.BluetoothSecurityCode,
                        VehicleNumber = e.VehicleNumber,
                        IsAccessEnabled = e.IsAccessEnabled
                    })
                    .ToListAsync();
                
                return accessEmployees;
            }
            
            return Forbid();
        }

        // GET: api/Employee/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(userId);
            
            var employee = await _context.Employees
                .Include(e => e.Division)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            // Check if user is in HR, GatePersonnel, or is a Manager of the employee's division
            bool canAccess = await _userManager.IsInRoleAsync(currentUser, "HR") ||
                             await _userManager.IsInRoleAsync(currentUser, "GatePersonnel") || // gatepersonnel just for testins
                             (await _userManager.IsInRoleAsync(currentUser, "Manager") &&
                              currentUser.DivisionId.HasValue &&
                              currentUser.DivisionId.Value == employee.DivisionId);

            
            if (!canAccess)
            {
                return Forbid();
            }

            var employeeDto = new EmployeeDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                CNP = employee.CNP,
                BadgeNumber = employee.BadgeNumber,
                PhotoUrl = employee.PhotoUrl,
                DivisionId = employee.DivisionId,
                DivisionName = employee.Division.Name,
                BluetoothSecurityCode = employee.BluetoothSecurityCode,
                VehicleNumber = employee.VehicleNumber,
                IsAccessEnabled = employee.IsAccessEnabled,
                ApprovalDate = employee.ApprovalDate
            };

            return employeeDto;
        }
        
        // POST: api/Employee
        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<ActionResult<Employee>> CreateEmployee(CreateEmployeeDto employeeDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var employee = new Employee
            {
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                CNP = employeeDto.CNP,
                BadgeNumber = employeeDto.BadgeNumber,
                PhotoUrl = employeeDto.PhotoUrl,
                DivisionId = employeeDto.DivisionId,
                BluetoothSecurityCode = employeeDto.BluetoothSecurityCode,
                VehicleNumber = employeeDto.VehicleNumber,
                IsAccessEnabled = true,
                ApprovedById = userId,
                ApprovalDate = DateTime.UtcNow
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        // PUT: api/Employee/5
        [HttpPut("{id}")]
        [Authorize(Roles = "HR,Manager")]
        public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeDto employeeDto)
        {
            if (id != employeeDto.Id)
            {
                return BadRequest();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(userId);
            
            // Manager can only update employees in their division and can't change division
            if (await _userManager.IsInRoleAsync(currentUser, "Manager"))
            {
                if (!currentUser.DivisionId.HasValue || employee.DivisionId != currentUser.DivisionId.Value)
                {
                    return Forbid();
                }
                
                // Managers can't change employee division
                if (employeeDto.DivisionId != employee.DivisionId)
                {
                    return BadRequest("Managers cannot change employee division");
                }
            }

            // HR can change all fields
            employee.FirstName = employeeDto.FirstName;
            employee.LastName = employeeDto.LastName;
            
            // Only HR can change these fields
            if (await _userManager.IsInRoleAsync(currentUser, "HR"))
            {
                employee.CNP = employeeDto.CNP;
                employee.BadgeNumber = employeeDto.BadgeNumber;
                employee.DivisionId = employeeDto.DivisionId;
            }
            
            // Both HR and Manager can update these fields
            employee.PhotoUrl = employeeDto.PhotoUrl;
            employee.BluetoothSecurityCode = employeeDto.BluetoothSecurityCode;
            employee.VehicleNumber = employeeDto.VehicleNumber;
            employee.IsAccessEnabled = employeeDto.IsAccessEnabled;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PATCH: api/Employee/5
        [HttpPatch("{id}")]
        [Authorize(Roles = "HR,Manager")]
        public async Task<IActionResult> EditEmployee(int id, [FromBody] UpdateEmployeeDto employeeDto)
        {
            if (id != employeeDto.Id)
            {
                return BadRequest();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(userId);
            
            // Manager can only update employees in their division and can't change division
            if (await _userManager.IsInRoleAsync(currentUser, "Manager"))
            {
                if (!currentUser.DivisionId.HasValue || employee.DivisionId != currentUser.DivisionId.Value)
                {
                    return Forbid();
                }
                
                // Managers can't change employee division
                if (employeeDto.DivisionId != employee.DivisionId)
                {
                    return BadRequest("Managers cannot change employee division");
                }
            }

            // Update allowed fields
            employee.FirstName = employeeDto.FirstName;
            employee.LastName = employeeDto.LastName;
            
            // Only HR can change these fields
            if (await _userManager.IsInRoleAsync(currentUser, "HR"))
            {
                employee.CNP = employeeDto.CNP;
                employee.BadgeNumber = employeeDto.BadgeNumber;
                employee.DivisionId = employeeDto.DivisionId;
            }
            
            // Both HR and Manager can update these fields
            employee.PhotoUrl = employeeDto.PhotoUrl;
            employee.BluetoothSecurityCode = employeeDto.BluetoothSecurityCode;
            employee.VehicleNumber = employeeDto.VehicleNumber;
            employee.IsAccessEnabled = employeeDto.IsAccessEnabled;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}