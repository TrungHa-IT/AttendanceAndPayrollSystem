using DataTransferObject.AttendanceDTO;
using DataTransferObject.EmployeeDTO;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using System.Drawing;

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

        [HttpPost]
        public async Task<IActionResult> CaculateSalary(int employeeId, int month, int year)
        {
            HttpClient client = new HttpClient();
            string apiUrl = $"https://localhost:7192/CaculateSalary/CalculateSalary?employeeId={employeeId}&month={month}&year={year}";

            HttpResponseMessage response = await client.PostAsync(apiUrl, null);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return Content(json, "application/json");
            }

            return StatusCode((int)response.StatusCode, "Lỗi khi gọi API tính lương");
        }

        [HttpGet]
        public async Task<IActionResult> ExportTimeSheetToExcel(int EmployId)
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

                var total = (item.CheckOut - item.CheckIn)?.TotalHours ?? 0;
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
