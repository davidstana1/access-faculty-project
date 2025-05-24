using backend.data;
using backend.dto.access_request;
using backend.entity;
using backend.enums;
using Microsoft.EntityFrameworkCore;

namespace backend.service.access.access_request;

 public class AccessRequestService : IAccessRequestService
    {
        private readonly ApplicationDbContext _dbContext;

        public AccessRequestService(
            ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<AccessRequestDto>> GetPendingRequestsAsync()
        {
            var pendingRequests = await _dbContext.AccessRequests
                .Where(r => r.Status == AccessRequestStatus.Pending)
                .OrderBy(r => r.Timestamp)
                .ToListAsync();

            return pendingRequests.Select(r => new AccessRequestDto
            {
                Id = r.Id.ToString(),
                EmployeeId = r.EmployeeId,
                Timestamp = r.Timestamp,
                Direction = r.Direction,
                Method = r.Method,
                VehicleNumber = r.VehicleNumber
            }).ToList();
        }

        public async Task<AccessRequestDto> CreateRequestAsync(CreateAccessRequestDto requestDto)
        {
            // Check if employee exists
            var employee = await _dbContext.Employees.FindAsync(requestDto.EmployeeId);
            if (employee == null)
            {
                throw new InvalidOperationException($"Employee with ID {requestDto.EmployeeId} not found");
            }

            // Create new access request
            var accessRequest = new AccessRequest
            {
                EmployeeId = requestDto.EmployeeId,
                Timestamp = DateTime.UtcNow,
                Direction = requestDto.Direction,
                Method = requestDto.Method,
                VehicleNumber = requestDto.VehicleNumber,
                Status = AccessRequestStatus.Pending
            };

            _dbContext.AccessRequests.Add(accessRequest);
            await _dbContext.SaveChangesAsync();

            // Return the created request with additional info
            return new AccessRequestDto
            {
                Id = accessRequest.Id.ToString(),
                EmployeeId = accessRequest.EmployeeId,
                Timestamp = accessRequest.Timestamp,
                Direction = accessRequest.Direction,
                Method = accessRequest.Method,
                VehicleNumber = accessRequest.VehicleNumber
            };
        }

        public async Task<bool> RemoveRequestAsync(string requestId)
        {
            if (!Guid.TryParse(requestId, out var id))
            {
                return false;
            }

            var request = await _dbContext.AccessRequests.FindAsync(id);
            if (request == null)
            {
                return false;
            }

            _dbContext.AccessRequests.Remove(request);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }