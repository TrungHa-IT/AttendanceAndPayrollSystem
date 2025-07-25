using BusinessObject.Models;
using ClosedXML.Excel;
using DataTransferObject.AttendanceDTO;
using DataTransferObject.DepartmentDTO;
using DataTransferObject.EmployeeDTOS;
using DataTransferObject.LeaveDTO;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.LeaveRepository;
using System.Text;
using System.Text.Json;

namespace FUNAttendanceAndPayrollSystemClient.Controllers.Staff
{
    public class StaffController : Controller
    {
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILeaveRepository repository = new LeaveRepository();
        private readonly FunattendanceAndPayrollSystemContext _context;
        private readonly HttpClient _client;

        public StaffController(IConfiguration configuration, FunattendanceAndPayrollSystemContext context, HttpClient client)
        {
            _baseUrl = configuration["ApiUrls:Base"]!;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _context = context;
            _client = client;
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> Payroll(int? month, int? year, int? department)
        {
            string urlDepart = $"{_baseUrl}/api/Department/getDepartment";
            string url = $"{_baseUrl}/Employee/getTotalHourByEmployee?month={month}&year={year}&departmenId={department}";

            var reponseDepartment = await _client.GetAsync(urlDepart);
            var departmentss = await reponseDepartment.Content.ReadAsStringAsync();

            var response = await _client.GetAsync(url);
            var employeeRes = await response.Content.ReadAsStringAsync();

            var departments = JsonSerializer.Deserialize<List<DepartmentDTO>>(departmentss, _jsonOptions);
            var employees = JsonSerializer.Deserialize<List<EmployeeDTO>>(employeeRes, _jsonOptions);

            ViewBag.departments = departments;
            ViewBag.Employees = employees;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ExportTimeSheetToExcel(int? month, int? year, int? department)
        {
            int currentMonth = month ?? DateTime.Now.Month;
            int currentYear = year ?? DateTime.Now.Year;

            string url = $"{_baseUrl}/Employee/getTotalHourByEmployee?month={currentMonth}&year={currentYear}";

            if (department.HasValue)
            {
                url += $"&departmenId={department.Value}";
            }

            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            HttpResponseMessage response = await _client.GetAsync(url);
            var strData = await response.Content.ReadAsStringAsync();

            List<EmployeeDTO> employees = JsonSerializer.Deserialize<List<EmployeeDTO>>(strData, option);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Timesheet");

            worksheet.Cell(1, 1).Value = "Employee ID";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Date of Birth";
            worksheet.Cell(1, 4).Value = "Email";
            worksheet.Cell(1, 5).Value = "Phone";
            worksheet.Cell(1, 6).Value = "Gender";
            worksheet.Cell(1, 7).Value = "Address";
            worksheet.Cell(1, 8).Value = "Position";
            worksheet.Cell(1, 9).Value = "Department";
            worksheet.Cell(1, 10).Value = "Total Payroll Hours";
            worksheet.Cell(1, 11).Value = "Total OT Hours";
            worksheet.Cell(1, 12).Value = "Salary (per hour)";
            worksheet.Cell(1, 13).Value = "Total Time Worked (hrs)";
            worksheet.Cell(1, 14).Value = "Total Salary";

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
                worksheet.Cell(row, 10).Value = item.Salary;
                worksheet.Cell(row, 11).Value = item.PayrollTime;
                worksheet.Cell(row, 10).Value = item.OvertimeTime;
                worksheet.Cell(row, 11).Value = Math.Round(item.TotalTimeWorked, 2);
                worksheet.Cell(row, 12).Value = Math.Round((decimal)(item.Salary ) * (decimal)item.TotalTimeWorked, 2);
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

        public async Task<IActionResult> ManageLeave()
        {
            using HttpClient client = new();

            var empId = HttpContext.Session.GetInt32("employeeId");
            if (empId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await client.GetAsync($"{_baseUrl}/api/Leave/getLeaveStaff?id={empId}");
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

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int leaveId, string status)
        {
            var empId = HttpContext.Session.GetInt32("employeeId");
            if (empId == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var employeeEmail = _context.Leaves.Include(e => e.Employee).Where(e => e.LeaveId == leaveId).FirstOrDefault();
            using HttpClient client = new();
 
            var leaveToUpdate = new
            {
                LeaveId = leaveId,
                Status = status,
                ApprovedBy = empId,
                EmployeeEmail = employeeEmail.Employee.Email
            };

            var content = new StringContent(
                JsonSerializer.Serialize(leaveToUpdate),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PutAsync($"{_baseUrl}/api/Leave/updateLeave", content);
            var errorContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Leave status updated successfully.";
            }
            else
            {
                TempData["Error"] = $"Failed to update leave status. API response: {response.StatusCode} - {errorContent}";
            }


            return RedirectToAction("ManageLeave");
        }

        public async Task<IActionResult> ManageCertificate()
        {
            using HttpClient client = new();

            var empId = HttpContext.Session.GetInt32("employeeId");
            if (empId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await client.GetAsync($"{_baseUrl}/Employee/GetCertificateByApprover?id={empId}");
            List<CertificateDTO> certificates = new();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(json))
                {
                    certificates = JsonSerializer.Deserialize<List<CertificateDTO>>(json, _jsonOptions) ?? new();
                }
            }

            return View(certificates);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCertificateStatus(int certificateId, string status)
        {
            var empId = HttpContext.Session.GetInt32("employeeId");
            if (empId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Lấy email nhân viên để gửi API
            var employeeCertificate = _context.EmployeeCertificates
                .Include(ec => ec.Employee)
                .Include(ec => ec.CertificateBonusRate)
                .FirstOrDefault(c => c.Id == certificateId);

            if (employeeCertificate == null)
            {
                TempData["Error"] = "Certificate not found.";
                return RedirectToAction("ManageCertificate");
            }

            var employeeEmail = employeeCertificate.Employee.Email;

            using HttpClient client = new();

            var certificateToUpdate = new
            {
                Id = certificateId,
                Status = status,
                ApprovedBy = empId,
                EmployeeEmail = employeeEmail
            };

            var content = new StringContent(
                JsonSerializer.Serialize(certificateToUpdate),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PutAsync($"{_baseUrl}/Employee/updateCertificate", content);
            var errorContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // ✅ Chỉ cập nhật lương nếu trạng thái là "Approved"
                if (status == "Approved" && employeeCertificate.CertificateBonusRate != null)
                {
                    var bonus = employeeCertificate.CertificateBonusRate.BonusAmount;
                    var employee = employeeCertificate.Employee;

                    employee.Salary += bonus;
                    employee.UpdatedAt = DateTime.Now;

                    _context.Employees.Update(employee);
                    await _context.SaveChangesAsync();
                }

                TempData["Message"] = "Certificate status updated successfully.";
            }
            else
            {
                TempData["Error"] = $"Failed to update certificate status. API response: {response.StatusCode} - {errorContent}";
            }

            return RedirectToAction("ManageCertificate");
        }


    }

}

