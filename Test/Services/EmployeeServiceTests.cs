using EmployeeApi.Models;
using EmployeeApi.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Xunit;

public class EmployeeServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memoryCache;
    private readonly EmployeeService _employeeService;

    public EmployeeServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _employeeService = new EmployeeService(_httpClient, _memoryCache);
    }

    [Fact]
    public async Task GetEmployeesAsync_ShouldReturnEmployees_WithAnnualSalaryCalculated()
    {
        // Arrange
        var fakeEmployees = new List<Employee>
        {
            new Employee { id = 1, employee_name = "Jefferson Linares", employee_salary = 1000, employee_age = 30, profile_image = "" },
            new Employee { id = 2, employee_name = "Fabian Ospina", employee_salary = 2000, employee_age = 29, profile_image = "" },
        };

        var fakeApiResponse = new ApiResponse { Data = fakeEmployees };
        var fakeJsonResponse = JsonConvert.SerializeObject(fakeApiResponse);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(fakeJsonResponse, Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _employeeService.GetEmployeesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(12000, result[0].anual_salary); // 1000 * 12
        Assert.Equal(24000, result[1].anual_salary); // 2000 * 12
    }

    [Fact]
    public async Task GetEmployeesAsync_ShouldUseCache_IfAvailable()
    {
        var cachedEmployees = new List<Employee>
        {
            new Employee { id = 1, employee_name = "Cached Employee", employee_salary = 1500, anual_salary = 18000 }
        };

        _memoryCache.Set("employees", cachedEmployees);

        var result = await _employeeService.GetEmployeesAsync();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Cached Employee", result[0].employee_name);
        Assert.Equal(18000, result[0].anual_salary);
    }
}