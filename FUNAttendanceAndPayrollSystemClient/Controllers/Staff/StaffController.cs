using BusinessObject.Models;
using ClosedXML.Excel;
using DataTransferObject.AttendanceDTO;
using DataTransferObject.EmployeeDTO;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FUNAttendanceAndPayrollSystemClient.Controllers.Staff
{
    public class StaffController : Controller
    {
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public StaffController(IConfiguration configuration)
        {
            _baseUrl = configuration["ApiUrls:Base"]!;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> Payroll(int? month, int? year)
        {
            HttpClient client = new HttpClient();

            string url = $"{_baseUrl}/Employee/getTotalHourByEmployee?month={month}&year={year}";

            var response = await client.GetAsync(url);
            var employeeRes = await response.Content.ReadAsStringAsync();
            var employees = JsonSerializer.Deserialize<List<EmployeeDTO>>(employeeRes, _jsonOptions);

            ViewBag.Employees = employees;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ExportTimeSheetToExcel(int? month, int? year)
        {
            HttpClient client = new HttpClient();
            int currentMonth = month ?? DateTime.Now.Month;
            int currentYear = year ?? DateTime.Now.Year;

            string url = $"{_baseUrl}/Employee/getTotalHourByEmployee?month={currentMonth}&year={currentYear}";

            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            HttpResponseMessage response = await client.GetAsync(url);
            var strData = await response.Content.ReadAsStringAsync();

            List<EmployeeDTO> employees = JsonSerializer.Deserialize<List<EmployeeDTO>>(strData, option);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Timesheet");

            // Header
            worksheet.Cell(1, 1).Value = "Employee ID";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Date of Birth";
            worksheet.Cell(1, 4).Value = "Email";
            worksheet.Cell(1, 5).Value = "Phone";
            worksheet.Cell(1, 6).Value = "Gender";
            worksheet.Cell(1, 7).Value = "Address";
            worksheet.Cell(1, 8).Value = "Position";
            worksheet.Cell(1, 9).Value = "Department";
            worksheet.Cell(1, 10).Value = "Salary (per hour)";
            worksheet.Cell(1, 11).Value = "Total Time Worked (hrs)";
            worksheet.Cell(1, 12).Value = "Total Salary";

            var headerRange = worksheet.Range("A1:L1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = 2;
            foreach (var item in employees)
            {
                worksheet.Cell(row, 1).Value = item.EmployId;
                worksheet.Cell(row, 2).Value = item.EmployeeName;
                worksheet.Cell(row, 3).Value = item.Dob?.ToString("yyyy-MM-dd") ?? "";
                worksheet.Cell(row, 4).Value = item.Email;
                worksheet.Cell(row, 5).Value = item.PhoneNumber;
                worksheet.Cell(row, 6).Value = item.Gender;
                worksheet.Cell(row, 7).Value = item.Address;
                worksheet.Cell(row, 8).Value = item.Position;
                worksheet.Cell(row, 9).Value = item.DepartmentName;
                worksheet.Cell(row, 10).Value = item.Salary ?? 0;
                worksheet.Cell(row, 11).Value = Math.Round(item.TotalTimeWorked, 2);
                worksheet.Cell(row, 12).Value = Math.Round((decimal)(item.Salary ?? 0) * (decimal)item.TotalTimeWorked, 2);
                row++;
            }

            worksheet.Columns().AdjustToContents();

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"Salary_{currentMonth}_{currentYear}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(stream, contentType, fileName);
        }
    }

}

