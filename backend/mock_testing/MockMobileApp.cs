// MockMobileApp.cs - Simulates requests from mobile app
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using backend.enums;
using Microsoft.Extensions.Hosting;

namespace backend.mock
{
    public class MockMobileApp : IHostedService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MockMobileApp> _logger;
        private Timer _timer;
        private readonly Random _random = new Random();
        
        // Predefined list of test employees to simulate requests from
        private readonly List<TestEmployee> _testEmployees = new List<TestEmployee>
        {
            new TestEmployee { Id = 11, FirstName = "John", LastName = "Doe", VehicleNumber = "B123ABC", Method = "Bluetooth" },
            new TestEmployee { Id = 2, FirstName = "Jane", LastName = "Smith", VehicleNumber = "B456DEF", Method = "Bluetooth" },
            new TestEmployee { Id = 3, FirstName = "Alex", LastName = "Johnson", VehicleNumber = null, Method = "Badge" },
            new TestEmployee { Id = 4, FirstName = "Maria", LastName = "Garcia", VehicleNumber = "CJ01XYZ", Method = "Bluetooth" },
        };

        public MockMobileApp(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<MockMobileApp> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Mock Mobile App Service started");
            
            // Simulate a mobile app request every 30 seconds
            _timer = new Timer(GenerateAccessRequest, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30));
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Mock Mobile App Service stopped");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async void GenerateAccessRequest(object state)
        {
            try
            {
                // Randomly select an employee
                var employee = _testEmployees[_random.Next(_testEmployees.Count)];
                
                // Randomly choose entry or exit
                var direction = _random.Next(2) == 0 ? AccessDirection.Entry : AccessDirection.Exit;
                
                var request = new
                {
                    EmployeeId = employee.Id,
                    Direction = direction.ToString(),
                    Method = employee.Method,
                    VehicleNumber = employee.VehicleNumber
                };
                
                _logger.LogInformation($"Creating mock access request for {employee.FirstName} {employee.LastName} ({direction})");
                
                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                // Send the access request to your API
                var response = await _httpClient.PostAsync("http://localhost:5203/api/accessRequests", content);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Mock access request created successfully");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to create mock access request: {response.StatusCode}, {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating mock access request");
            }
        }
        
        private class TestEmployee
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string VehicleNumber { get; set; }
            public string Method { get; set; }
        }
    }
}