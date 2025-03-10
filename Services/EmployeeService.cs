using EmployeeApi.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Text.Json;

namespace EmployeeApi.Services
{
    public class EmployeeService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private const string BASE_API_URL = "http://dummy.restapiexample.com/";

        public EmployeeService(HttpClient httpClient, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
        }

        private Employee CalculateAnnualSalary(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            employee.anual_salary = employee.employee_salary * 12;
            return employee;
        }

        // Obtiene la lista de empleados con caché para reducir peticiones
        public async Task<List<Employee>?> GetEmployeesAsync()
        {
            try
            {
                // Verifica si ya hay datos en caché
                if (_memoryCache.TryGetValue("employees", out List<Employee> cachedEmployees))
                {
                    return cachedEmployees;
                }

                const string API_URL = BASE_API_URL + "api/v1/employees";
                var employees = await SendRequestWithRetryAsync<ApiResponse>(API_URL);

                if (employees?.Data != null)
                {
                    foreach (var employee in employees.Data)
                    {
                        CalculateAnnualSalary(employee);
                    }

                    // Guarda en caché por cinco minutos
                    _memoryCache.Set("employees", employees.Data, TimeSpan.FromMinutes(5));
                }

                return employees?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo empleados: {ex.Message}");
                return null;
            }
        }

        //  Obtiene un solo empleado, usando caché o consulta a la API con reintentos.
        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            try
            {
                // Intentamos obtener la lista de empleados desde la caché
                if (!_memoryCache.TryGetValue("employees", out List<Employee>? cachedEmployees) || cachedEmployees == null)
                {
                    // Si no hay caché, obtenemos todos los empleados de la API
                    var employees = await GetEmployeesAsync();

                    // Si la API devolvió 429, no intentamos más peticiones y retornamos null
                    if (employees == null)
                    {
                        Console.WriteLine("Se ha excedido el límite de peticiones.");
                        return null;
                    }

                    // Guardamos la lista en caché por 10 minutos
                    _memoryCache.Set("employees", employees, TimeSpan.FromMinutes(5));
                    cachedEmployees = employees;
                }

                // Filtrar al empleado por ID desde la lista en caché
                var employee = cachedEmployees.FirstOrDefault(e => e.id == id);

                return employee != null ? CalculateAnnualSalary(employee) : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la consulta de empleado: {ex.Message}");
                return null;
            }
        }

        // Maneja peticiones HTTP con reintentos en caso de error 429 (Too Many Requests).
        private async Task<T?> SendRequestWithRetryAsync<T>(string url, int maxRetries = 5)
        {
            int delay = 1000; // 1 segundo inicial

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.Add("cookie", "humans_21909=1");

                    var response = await _httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<T>(json);
                    }
                    else if (response.StatusCode == HttpStatusCode.TooManyRequests) // Código 429
                    {
                        Console.WriteLine($"Límite alcanzado. Reintentando en {delay} ms...");
                        await Task.Delay(delay);
                        delay *= 2; // Duplica el tiempo de espera
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode}");
                        return default;
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error en la solicitud HTTP: {ex.Message}");
                }
            }

            Console.WriteLine("Se agotaron los intentos.");
            return default;
        }
    }

    public class ApiResponse
    {
        public List<Employee>? Data { get; set; }
    }

    public class EmployeesResponse
    {
        public string Status { get; set; } = "";
        public Employee Data { get; set; }
        public string Message { get; set; } = "";
    }
}