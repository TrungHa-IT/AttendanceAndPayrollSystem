using DataTransferObject.AttendanceDTO;
using DataTransferObject.EmployeeDTOS;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using System.Drawing;

using System.Text.Json;
using DataTransferObject.DepartmentDTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FUNAttendanceAndPayrollSystemClient.Controllers.Staff
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

        public async Task<IActionResult> ManageAttendance(string name, int? department)
        {
            string baseUrl = _configuration["ApiUrls:Base"];
            using var client = new HttpClient();

            string url = $"{baseUrl}/Employee/ListEmployees";
            var response = await client.GetAsync(url);
            var strData = await response.Content.ReadAsStringAsync();
            var employees = JsonSerializer.Deserialize<List<EmployeeDTO>>(strData, _jsonOptions);

            if (!string.IsNullOrEmpty(name))
            {
                employees = employees
                    .Where(e => !string.IsNullOrEmpty(e.EmployeeName) && e.EmployeeName.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (department.HasValue && department.Value != 0)
            {
                employees = employees
                    .Where(e => e.DepartmentId == department.Value)
                    .ToList();
            }

            string urlDepart = $"{baseUrl}/api/Department/getDepartment";
            var responseDepartment = await client.GetAsync(urlDepart);
            var departmentData = await responseDepartment.Content.ReadAsStringAsync();
            var departments = JsonSerializer.Deserialize<List<DepartmentDTO>>(departmentData, _jsonOptions);

            ViewBag.departments = departments;
            ViewBag.SelectedDepartment = department?.ToString();
            ViewBag.SearchName = name;
            ViewBag.Employees = employees;

            return View();
        }

        public async Task<IActionResult> ShowTimeSheet(int? EmployId = null, int? month = null, int? year = null)
        {
            string baseUrl = _configuration["ApiUrls:Base"];
            using var client = new HttpClient();

            string role = HttpContext.Session.GetString("role")?.ToLower();
            int? sessionEmployeeId = HttpContext.Session.GetInt32("employeeId");

            int employeeIdToUse = role == "staff"
                ? (EmployId ?? sessionEmployeeId ?? 0)
                : sessionEmployeeId ?? 0;

            int selectedMonth = month ?? DateTime.Now.Month;
            int selectedYear = year ?? DateTime.Now.Year;

            string url = $"{baseUrl}/Employee/MyAttendance?emp={employeeIdToUse}&month={selectedMonth}&year={selectedYear}";
            var response = await client.GetAsync(url);
            var strData = await response.Content.ReadAsStringAsync();

            var attendances = JsonSerializer.Deserialize<List<AttendanceDTO>>(strData, _jsonOptions);

            ViewBag.Attendances = attendances ?? new();
            ViewBag.Role = role;
            ViewBag.SelectedEmployeeId = employeeIdToUse;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CaculateSalary([FromQuery] int employeeId, [FromQuery] int month, [FromQuery] int year)
        {
            HttpClient client = new HttpClient();
            string baseUrl = _configuration["ApiUrls:Base"];
            string apiUrl = $"{baseUrl}/CaculateSalary/CalculateSalary?employeeId={employeeId}&month={month}&year={year}";

            HttpResponseMessage response = await client.PostAsync(apiUrl, null);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return Content(json, "application/json");
            }

            return StatusCode((int)response.StatusCode, "Lỗi khi gọi API tính lương");
        }

        [HttpGet]
        public async Task<IActionResult> ExportTimeSheetToExcel(int EmployId, int? month = null, int? year = null)
        {
            HttpClient client = new HttpClient();
            string baseUrl = _configuration["ApiUrls:Base"];
            string url = $"{baseUrl}/Employee/MyAttendance?emp={EmployId}";

            // Append month and year if provided
            if (month.HasValue && year.HasValue)
            {
                url += $"&month={month.Value}&year={year.Value}";
            }

            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            HttpResponseMessage response = await client.GetAsync(url);
            var strData = await response.Content.ReadAsStringAsync();

            List<AttendanceDTO> attendances = JsonSerializer.Deserialize<List<AttendanceDTO>>(strData, option);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Timesheet");

            worksheet.Cell(1, 1).Value = "Employee ID";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Department";
            worksheet.Cell(1, 4).Value = "Work Date";
            worksheet.Cell(1, 5).Value = "Check In";
            worksheet.Cell(1, 6).Value = "Check Out";
            worksheet.Cell(1, 7).Value = "Total Time (hrs)";

            var headerRange = worksheet.Range("A1:G1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = 2;
            foreach (var item in attendances)
            {
                worksheet.Cell(row, 1).Value = item.EmployeeId;
                worksheet.Cell(row, 2).Value = item.EmployeeName;
                worksheet.Cell(row, 3).Value = item.DepartmentName;
                worksheet.Cell(row, 4).Value = item.WorkDate.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 5).Value = item.CheckIn.ToString("HH:mm");
                worksheet.Cell(row, 6).Value = item.CheckOut?.ToString("HH:mm") ?? "-";

                var total = (item.CheckOut - item.CheckIn - TimeSpan.FromHours(1))?.TotalHours ?? 0;
                worksheet.Cell(row, 7).Value = Math.Round((decimal)total, 2);

                row++;
            }

            worksheet.Columns().AdjustToContents();

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var employeeName = attendances.FirstOrDefault()?.EmployeeName ?? "Employee";
            var sanitizedEmployeeName = string.Concat(employeeName.Split(Path.GetInvalidFileNameChars()));
            var fileName = $"Timesheet_{sanitizedEmployeeName}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(stream, contentType, fileName);
        }

    }

}
