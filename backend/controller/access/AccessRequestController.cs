using backend.data;
using backend.dto.access_request;
using backend.service.access.access_request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.controller;

[Route("api/accessRequests")]
[ApiController]
public class AccessRequestController : ControllerBase
{
    private readonly IAccessRequestService _accessRequestService;
    private readonly ApplicationDbContext _context;

    public AccessRequestController(IAccessRequestService accessRequestService,ApplicationDbContext context)
    {
        _accessRequestService = accessRequestService;
        _context = context;
    }

    // GET: api/accessRequests/pending
    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<AccessRequestDto>>> GetPendingRequests()
    {
        var pendingRequests = await _accessRequestService.GetPendingRequestsAsync();
        return Ok(pendingRequests);
    }
    
    [HttpGet("latest")]
    public async Task<IActionResult> GetLatestRequest()
    {
        var latestRequest = await _context.AccessRequests
            .OrderByDescending(r => r.Timestamp)
            .FirstOrDefaultAsync();

        if (latestRequest == null)
            return NotFound();

        return Ok(latestRequest);
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