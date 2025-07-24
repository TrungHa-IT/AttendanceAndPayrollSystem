using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http.Headers;
using DataTransferObject.EmployeeDTOS;
using DataTransferObject.EmployeeDTO;

namespace FUNAttendanceAndPayrollSystemClient.Controllers.Employee
{
    public class CertificateController : Controller
    {
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public CertificateController(IConfiguration configuration)
        {
            _baseUrl = configuration["ApiUrls:Base"]!;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IActionResult> Index()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{_baseUrl}/Employee/GetCertificate");

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<CertificateDTO>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<CertificateDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(list ?? new List<CertificateDTO>());
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

            ////Email Employee
            //var employeeEmail = HttpContext.Session.GetString("email");
            //ViewBag.EmployeeEmail = employeeEmail;
            // Leave Types
            var leaveTypeRes = await client.GetAsync($"{_baseUrl}/Employee/ListCertificateBonus");
            List<CertificateBonusDTO> leaveTypes = new();
            if (leaveTypeRes.IsSuccessStatusCode)
            {
                var leaveTypeJson = await leaveTypeRes.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(leaveTypeJson))
                {
                    leaveTypes = JsonSerializer.Deserialize<List<CertificateBonusDTO>>(leaveTypeJson, _jsonOptions) ?? new List<CertificateBonusDTO>();
                }
            }
            ViewBag.ListCertificate = leaveTypes;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CertificateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using var client = new HttpClient();
            using var form = new MultipartFormDataContent();

            // Gán employeeId từ session
            model.EmployeeId = HttpContext.Session.GetInt32("employeeId") ?? 0;

            // 🟩 Add normal properties
            form.Add(new StringContent(model.EmployeeId.ToString()), "EmployeeId");
            form.Add(new StringContent(model.CertificateName ?? ""), "CertificateName");
            form.Add(new StringContent(model.IssueDate.ToString("o")), "IssueDate");
            form.Add(new StringContent(model.ExpiryDate.ToString("o")), "ExpiryDate");
            form.Add(new StringContent(model.Status ?? ""), "Status");

            if (model.ApprovedBy != null)
                form.Add(new StringContent(model.ApprovedBy.ToString()), "ApprovedBy");
            if (model.CertificateBonusRateId != null)
                form.Add(new StringContent(model.CertificateBonusRateId.ToString()), "CertificateBonusRateId");
            if (model.BonusRate != null)
                form.Add(new StringContent(model.BonusRate.ToString()), "BonusRate");

            // 🟨 Add image files
            if (model.ImageFiles != null && model.ImageFiles.Any())
            {
                foreach (var file in model.ImageFiles)
                {
                    if (file.Length > 0)
                    {
                        var streamContent = new StreamContent(file.OpenReadStream());
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                        form.Add(streamContent, "ImageFiles", file.FileName); // 🔑 key phải đúng tên property bên DTO
                    }
                }
            }

            // 🟥 Gửi request đúng định dạng multipart
            var response = await client.PostAsync($"{_baseUrl}/Employee/AddCertificate", form);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "Failed to create certificate. API response: " + error);
            return View(model);
        }


    }
}
