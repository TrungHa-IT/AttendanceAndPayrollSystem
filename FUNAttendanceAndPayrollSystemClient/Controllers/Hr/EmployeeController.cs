using DataTransferObject.DepartmentDTO;
using DataTransferObject.EmployeeDTO;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FUNAttendanceAndPayrollSystemClient.Controllers.Hr
{
    public class EmployeeController : Controller
    {
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public EmployeeController(IConfiguration configuration)
        {
            _baseUrl = configuration["ApiUrls:Base"]!;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public async Task<IActionResult> Employee()
        {
            using HttpClient client = new();
            var employeeRes = await client.GetAsync($"{_baseUrl}/api/Employee/ListEmployees");
            var employeeJson = await employeeRes.Content.ReadAsStringAsync();
            var employees = JsonSerializer.Deserialize<List<EmployeeDTO>>(employeeJson, _jsonOptions);
            return View("~/Views/Hr/Employee.cshtml", employees);
        }
    }
}
