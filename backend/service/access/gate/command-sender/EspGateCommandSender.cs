using System.Text;
using System.Text.Json;
using backend.entity;
using backend.enums;

namespace backend.service.access.gate.command_sender;

public class EspGateCommandSender : IGateCommandSender
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EspGateCommandSender> _logger;
    private readonly string _gateControllerUrl;

    public EspGateCommandSender(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<EspGateCommandSender> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _gateControllerUrl = configuration["GateController:Url"];
    }

    public async Task<GateStatus> GetStatusAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_gateControllerUrl}/status");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GateStatus>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            _logger.LogWarning("Failed to get gate status. Status code: {StatusCode}", response.StatusCode);
            return new GateStatus
            {
                State = "unknown",
                IsOperational = false,
                LastOperation = "status check failed",
                LastOperationTime = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting gate status");
            return new GateStatus
            {
                State = "error",
                IsOperational = false,
                LastOperation = $"Error: {ex.Message}",
                LastOperationTime = DateTime.UtcNow
            };
        }
    }

    public async Task<GateOperationResult> UpdateStateAsync(string state)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { state }),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_gateControllerUrl}/state", content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GateOperationResult>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            _logger.LogWarning("Failed to update gate state. Status code: {StatusCode}", response.StatusCode);
            return new GateOperationResult
            {
                Success = false,
                Message = $"Failed to update state. Status code: {response.StatusCode}",
                NewState = "unknown"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating gate state");
            return new GateOperationResult
            {
                Success = false,
                Message = $"Error: {ex.Message}",
                NewState = "error"
            };
        }
    }

    public async Task<GateOperationResult> OpenGateAsync()
    {
        try
        {
            var response = await _httpClient.PostAsync(
                $"{_gateControllerUrl}/open",
                new StringContent("{}", Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GateOperationResult>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            _logger.LogWarning("Failed to open gate. Status code: {StatusCode}", response.StatusCode);
            return new GateOperationResult
            {
                Success = false,
                Message = $"Failed to open gate. Status code: {response.StatusCode}",
                NewState = "unknown"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while opening gate");
            return new GateOperationResult
            {
                Success = false,
                Message = $"Error: {ex.Message}",
                NewState = "error"
            };
        }
    }

    public async Task<GateOperationResult> CloseGateAsync()
    {
        try
        {
            var response = await _httpClient.PostAsync(
                $"{_gateControllerUrl}/close",
                new StringContent("{}", Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GateOperationResult>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            _logger.LogWarning("Failed to close gate. Status code: {StatusCode}", response.StatusCode);
            return new GateOperationResult
            {
                Success = false,
                Message = $"Failed to close gate. Status code: {response.StatusCode}",
                NewState = "unknown"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while closing gate");
            return new GateOperationResult
            {
                Success = false,
                Message = $"Error: {ex.Message}",
                NewState = "error"
            };
        }
    }
}