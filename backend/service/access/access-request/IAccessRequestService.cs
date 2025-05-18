using backend.dto.access_request;

namespace backend.service.access.access_request;

public interface IAccessRequestService
{
    Task<List<AccessRequestDto>> GetPendingRequestsAsync();
    Task<AccessRequestDto> CreateRequestAsync(CreateAccessRequestDto requestDto);
    Task<bool> RemoveRequestAsync(string requestId);
}