// MockEspGateDevice.cs - Simulates ESP32 hardware responses
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using backend.entity;
using backend.enums;
using backend.service.access.gate;

namespace backend.mock
{
    public class MockEspGateDevice
    {
        private const string GATE_STATUS_PATH = "/status";
        private const string GATE_STATE_PATH = "/state";
        private const string GATE_OPEN_PATH = "/open";
        private const string GATE_CLOSE_PATH = "/close";
        
        private readonly ILogger<MockEspGateDevice> _logger;
        private GateStatus _currentStatus;
        private readonly object _statusLock = new object();

        public MockEspGateDevice(ILogger<MockEspGateDevice> logger)
        {
            _logger = logger;
            // Initialize with a closed gate
            _currentStatus = new GateStatus
            {
                State = "closed",
                IsOperational = true,
                LastOperation = "initialized",
                LastOperationTime = DateTime.UtcNow
            };
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // GET /status - Get current gate status
                endpoints.MapGet(GATE_STATUS_PATH, HandleGetStatus);
                
                // POST /state - Update gate state
                endpoints.MapPost(GATE_STATE_PATH, HandleUpdateState);
                
                // POST /open - Open the gate
                endpoints.MapPost(GATE_OPEN_PATH, HandleOpenGate);
                
                // POST /close - Close the gate
                endpoints.MapPost(GATE_CLOSE_PATH, HandleCloseGate);
            });
        }

        private async Task HandleGetStatus(HttpContext context)
        {
            _logger.LogInformation("Handling GET status request");
            await WriteJsonResponse(context, _currentStatus);
        }

        private async Task HandleUpdateState(HttpContext context)
        {
            _logger.LogInformation("Handling POST state request");
            
            try
            {
                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var requestData = JsonSerializer.Deserialize<JsonElement>(body);
                
                if (requestData.TryGetProperty("state", out var stateElement))
                {
                    var newState = stateElement.GetString();
                    
                    lock (_statusLock)
                    {
                        _currentStatus.State = newState;
                        _currentStatus.LastOperation = $"state changed to {newState}";
                        _currentStatus.LastOperationTime = DateTime.UtcNow;
                    }
                    
                    var result = new GateOperationResult
                    {
                        Success = true,
                        Message = $"State updated to {newState}",
                        NewState = newState
                    };
                    
                    await WriteJsonResponse(context, result);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await WriteJsonResponse(context, new { error = "Missing state parameter" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing state update request");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await WriteJsonResponse(context, new { error = ex.Message });
            }
        }

        private async Task HandleOpenGate(HttpContext context)
        {
            _logger.LogInformation("Handling POST open gate request");
            
            // Simulate gate opening process
            lock (_statusLock)
            {
                _currentStatus.State = "opening";
                _currentStatus.LastOperation = "opening gate";
                _currentStatus.LastOperationTime = DateTime.UtcNow;
            }
            
            // Simulate gate opening process taking time
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000); // 3 seconds to open
                
                lock (_statusLock)
                {
                    _currentStatus.State = "open";
                    _currentStatus.LastOperation = "gate opened";
                    _currentStatus.LastOperationTime = DateTime.UtcNow;
                }
                
                _logger.LogInformation("Gate is now fully open");
            });
            
            var result = new GateOperationResult
            {
                Success = true,
                Message = "Gate is opening",
                NewState = "opening"
            };
            
            await WriteJsonResponse(context, result);
        }

        private async Task HandleCloseGate(HttpContext context)
        {
            _logger.LogInformation("Handling POST close gate request");
            
            // Simulate gate closing process
            lock (_statusLock)
            {
                _currentStatus.State = "closing";
                _currentStatus.LastOperation = "closing gate";
                _currentStatus.LastOperationTime = DateTime.UtcNow;
            }
            
            // Simulate gate closing process taking time
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000); // 3 seconds to close
                
                lock (_statusLock)
                {
                    _currentStatus.State = "closed";
                    _currentStatus.LastOperation = "gate closed";
                    _currentStatus.LastOperationTime = DateTime.UtcNow;
                }
                
                _logger.LogInformation("Gate is now fully closed");
            });
            
            var result = new GateOperationResult
            {
                Success = true,
                Message = "Gate is closing",
                NewState = "closing"
            };
            
            await WriteJsonResponse(context, result);
        }

        private async Task WriteJsonResponse(HttpContext context, object data)
        {
            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.Body, data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }

    // Extension method to easily add the mock ESP service
    public static class MockEspGateDeviceExtensions
    {
        public static IServiceCollection AddMockEspGateDevice(this IServiceCollection services)
        {
            services.AddSingleton<MockEspGateDevice>();
            return services;
        }
        
        public static IApplicationBuilder UseMockEspGateDevice(this IApplicationBuilder app)
        {
            var mockDevice = app.ApplicationServices.GetRequiredService<MockEspGateDevice>();
            mockDevice.Configure(app);
            return app;
        }
    }
}