using backend.entity;
using backend.service.access.gate.command_sender;

namespace backend.service.access.gate;

public class GateService : IGateService
{
    private readonly IGateCommandSender _gateCommandSender;

    public GateService(IGateCommandSender gateCommandSender)
    {
        _gateCommandSender = gateCommandSender;
    }

    public async Task<GateStatus> GetCurrentStatusAsync()
    {
        // Get status from the gate control hardware (ESP device)
        var status = await _gateCommandSender.GetStatusAsync();
        return status;
    }

    public async Task<GateOperationResult> UpdateGateStateAsync(string state)
    {
        // Validate the state is a valid transition
        if (state != "open" && state != "closed" && state != "opening" && state != "closing")
        {
            return new GateOperationResult
            {
                Success = false,
                Message = "Invalid gate state requested",
                NewState = (await GetCurrentStatusAsync()).State
            };
        }

        // Send update command to gate control hardware
        var result = await _gateCommandSender.UpdateStateAsync(state);
        return result;
    }

    public async Task<GateOperationResult> OpenGateAsync()
    {
        // Logic to open the gate - send command to ESP device
        var result = await _gateCommandSender.OpenGateAsync();
        return result;
    }

    public async Task<GateOperationResult> CloseGateAsync()
    {
        // Logic to close the gate - send command to ESP device
        var result = await _gateCommandSender.CloseGateAsync();
        return result;
    }
}
