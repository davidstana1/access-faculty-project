using backend.dto.access_request;
using backend.service.access.access_request;
using Microsoft.AspNetCore.Mvc;

namespace backend.controller;

[Route("api/accessRequests")]
[ApiController]
public class AccessRequestController : ControllerBase
{
    private readonly IAccessRequestService _accessRequestService;

    public AccessRequestController(IAccessRequestService accessRequestService)
    {
        _accessRequestService = accessRequestService;
    }

    // GET: api/accessRequests/pending
    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<AccessRequestDto>>> GetPendingRequests()
    {
        var pendingRequests = await _accessRequestService.GetPendingRequestsAsync();
        return Ok(pendingRequests);
    }

    // POST: api/accessRequests
    [HttpPost]
    public async Task<ActionResult<AccessRequestDto>> CreateRequest([FromBody] CreateAccessRequestDto requestDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdRequest = await _accessRequestService.CreateRequestAsync(requestDto);
        return CreatedAtAction(nameof(GetPendingRequests), new { id = createdRequest.Id }, createdRequest);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<AccessRequestDto>> DeleteRequest(string id)
    {
        var result = await _accessRequestService.RemoveRequestAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return Ok(result);
    }
}