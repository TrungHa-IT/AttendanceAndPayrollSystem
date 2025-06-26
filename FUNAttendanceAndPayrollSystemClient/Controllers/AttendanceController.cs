using DataTransferObject.AttendanceDTO;
using DataTransferObject.EmployeeDTO;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FUNAttendanceAndPayrollSystemClient.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerOptions _jsonOptions;

        public AttendanceController(IConfiguration configuration)
        {
            _configuration = configuration;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<IActionResult> ManageAttendance()
        {
            string baseUrl = _configuration["ApiUrls:Base"];
            using var client = new HttpClient();

            string url = $"{baseUrl}/Employee/ListEmployees";
            var response = await client.GetAsync(url);
            var strData = await response.Content.ReadAsStringAsync();
            var employees = JsonSerializer.Deserialize<List<EmployeeDTO>>(strData, _jsonOptions);

            ViewBag.Employees = employees;
            return View();
        }

        public async Task<IActionResult> ShowTimeSheet(int EmployId)
        {
            string baseUrl = _configuration["ApiUrls:Base"];
            using var client = new HttpClient();

            string url = $"{baseUrl}/Employee/MyAttendance?emp={EmployId}";
            var response = await client.GetAsync(url);
            var strData = await response.Content.ReadAsStringAsync();
            var attendances = JsonSerializer.Deserialize<List<AttendanceDTO>>(strData, _jsonOptions);

            ViewBag.attendances = attendances;
            return View();
        }
    }
}
