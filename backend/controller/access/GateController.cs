using backend.dto.access_request;
using backend.enums;
using backend.service.access.access_request;
using backend.service.access.gate;
using Microsoft.AspNetCore.Mvc;

namespace backend.controller;

[Route("api/[controller]")]
[ApiController]
public class GateController : ControllerBase
{
    private readonly IGateService _gateService;

    public GateController(IGateService gateService)
    {
        _gateService = gateService;
    }

    // GET: api/gate/status
    [HttpGet("status")]
    public async Task<IActionResult> GetGateStatus()
    {
        var status = await _gateService.GetCurrentStatusAsync();
        return Ok(status);
    }

    // POST: api/gate/state
    [HttpPost("state")]
    public async Task<IActionResult> UpdateGateState([FromBody] GateStateUpdateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _gateService.UpdateGateStateAsync(request.State);
        return Ok(result);
    }

    // POST: api/gate/open
    [HttpPost("open")]
    public async Task<IActionResult> OpenGate()
    {
        var result = await _gateService.OpenGateAsync();
        return Ok(result);
    }

    // POST: api/gate/close
    [HttpPost("close")]
    public async Task<IActionResult> CloseGate()
    {
        var result = await _gateService.CloseGateAsync();
        return Ok(result);
    }
}

public class GateStateUpdateRequest
{
    public string State { get; set; }
}