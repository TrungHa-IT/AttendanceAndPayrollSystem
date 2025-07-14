using DataTransferObject.DepartmentDTO;
using DataTransferObject.EmployeeDTO;
using DataTransferObject.LeaveDTO;
using DataTransferObject.LeaveTypeDTO;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FUNAttendanceAndPayrollSystemClient.Controllers.Employee
{
    public class LeaveController : Controller
    {
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public LeaveController(IConfiguration configuration)
        {
            _baseUrl = configuration["ApiUrls:Base"]!;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IActionResult> Create()
        {
            using HttpClient client = new();

            // Staffs
            var staffRes = await client.GetAsync($"{_baseUrl}/Employee/ListStaff");
            List<EmployeeDTO> staffs = new();
            if (staffRes.IsSuccessStatusCode)
            {
                var staffJson = await staffRes.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(staffJson))
                {
                    staffs = JsonSerializer.Deserialize<List<EmployeeDTO>>(staffJson, _jsonOptions) ?? new List<EmployeeDTO>();
                }
            }
            ViewBag.Staff = staffs;

            // Leave Types
            var leaveTypeRes = await client.GetAsync($"{_baseUrl}/api/LeaveType/getLeaveType");
            List<LeaveTypeDTO> leaveTypes = new();
            if (leaveTypeRes.IsSuccessStatusCode)
            {
                var leaveTypeJson = await leaveTypeRes.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(leaveTypeJson))
                {
                    leaveTypes = JsonSerializer.Deserialize<List<LeaveTypeDTO>>(leaveTypeJson, _jsonOptions) ?? new List<LeaveTypeDTO>();
                }
            }
            ViewBag.LeaveTypes = leaveTypes;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(DataTransferObject.LeaveDTO.LeaveDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using HttpClient client = new();
            model.CreatedAt = DateTime.Now;
            model.EmployeeId = HttpContext.Session.GetInt32("employeeId") ?? 0; 
            model.DurationInDays = model.DurationInDays = (decimal)(model.EndDate - model.StartDate).TotalDays;
            model.ApprovedDate = null;
            model.UpdatedAt = null;
            var content = new StringContent(
                JsonSerializer.Serialize(model),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync($"{_baseUrl}/api/Leave/createLeave", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Leave"); 
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", "Failed to create leave. API response: " + error);
                return View(model);
            }
        }

        public async Task<IActionResult> Index()
        {
            using HttpClient client = new();

            var empId = HttpContext.Session.GetInt32("employeeId");
            if (empId == null)
            {
                return RedirectToAction("Login", "Auth"); 
            }

            var response = await client.GetAsync($"{_baseUrl}/api/Leave/getLeaveEmployee?id={empId}");
            List<LeaveDTO> leaves = new();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(json))
                {
                    leaves = JsonSerializer.Deserialize<List<LeaveDTO>>(json, _jsonOptions) ?? new();
                }
            }

            return View(leaves);
        }


    }
}
