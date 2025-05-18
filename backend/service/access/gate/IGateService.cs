using backend.entity;
using backend.enums;

namespace backend.service.access.gate;

public interface IGateService
{
    Task<GateStatus> GetCurrentStatusAsync();
    Task<GateOperationResult> UpdateGateStateAsync(string state);
    Task<GateOperationResult> OpenGateAsync();
    Task<GateOperationResult> CloseGateAsync();
}

public class GateOperationResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string NewState { get; set; }
}