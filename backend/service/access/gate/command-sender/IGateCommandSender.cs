using backend.entity;
using backend.enums;

namespace backend.service.access.gate.command_sender;

public interface IGateCommandSender
{
    Task<GateStatus> GetStatusAsync();
    Task<GateOperationResult> UpdateStateAsync(string state);
    Task<GateOperationResult> OpenGateAsync();
    Task<GateOperationResult> CloseGateAsync();
}