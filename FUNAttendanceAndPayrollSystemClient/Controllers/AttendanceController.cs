using DataTransferObject.AttendanceDTO;
using DataTransferObject.EmployeeDTO;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FUNAttendanceAndPayrollSystemClient.Controllers
{
    public class AttendanceController : Controller
    {
        public async Task<IActionResult> ManageAttendance()
        {
            HttpClient client = new HttpClient();
            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string url = "https://localhost:7192/Employee/ListEmployees";
            HttpResponseMessage response = await client.GetAsync(url);
            var strData = await response.Content.ReadAsStringAsync();
            List<EmployeeDTO> employees = JsonSerializer.Deserialize<List<EmployeeDTO>>(strData, option);
            ViewBag.Employees = employees;
            return View();
        }

        public async Task<IActionResult> ShowTimeSheet(int EmployId)
        {
            HttpClient client = new HttpClient();
            string url = $"https://localhost:7192/Employee/MyAttendance?emp={EmployId}";
            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            HttpResponseMessage response = await client.GetAsync(url);
            var strData = await response.Content.ReadAsStringAsync();
            List<AttendanceDTO> attendances = JsonSerializer.Deserialize<List<AttendanceDTO>>(strData, option);
            ViewBag.attendances = attendances;
            return View();
        }
    }
}
