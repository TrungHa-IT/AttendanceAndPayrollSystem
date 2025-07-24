using DataTransferObject.EmployeeDTOS;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace FUNAttendanceAndPayrollSystemClient.Controllers.Employee
{
    public class EmployeeController : Controller
    {
        public IActionResult Dashboard()
        {
            int? employeeId = HttpContext.Session.GetInt32("employeeId");
            ViewBag.EmployeeId = employeeId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterTimeOT(OTRequestDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Reason))
            {
                TempData["Error"] = "Reason is required.";
                return RedirectToAction("Dashboard");
            }

            var today = DateOnly.FromDateTime(DateTime.Now);
            if (model.OtDate <= today)
            {
                TempData["Error"] = "Please select a date after today.";
                return RedirectToAction("Dashboard");
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7192");

                var response = await client.PostAsJsonAsync("/Employee/RegisterScheduleOT", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "OT registered successfully.";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = "Error: " + error;
                    return RedirectToAction("Dashboard");
                }
            }
        }

        public async Task<IActionResult> Profile()
        {
            int? employeeId = HttpContext.Session.GetInt32("employeeId");
            HttpClient client = new HttpClient();
            string url = $"https://localhost:7192/Employee/Profile/{employeeId}";
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var jsonOprtion = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var informations = JsonSerializer.Deserialize<EmployeeProfileDTO>(content, jsonOprtion);
            ViewBag.informations = informations;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBasicInfos(EmployeeProfileDTO dto)
        {
            int? employeeId = HttpContext.Session.GetInt32("employeeId");
            if (employeeId == null)
            {
                TempData["Error"] = "Session expired.";
                return RedirectToAction("Login");
            }

            dto.EmployId = employeeId.Value; 
            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("https://localhost:7192/Employee/UpdateBasicInfo", content);

            TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                response.IsSuccessStatusCode ? "Updated!" : "Update failed.";

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSkills(List<SkillDTO> skills)
        {
            int? employeeId = HttpContext.Session.GetInt32("employeeId");
            if (employeeId == null)
            {
                TempData["Error"] = "Session expired.";
                return RedirectToAction("Login");
            }

            var request = new
            {
                EmployeeId = employeeId.Value,
                Skills = skills
            };

            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7192/Employee/UpdateSkills", content);

            TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                response.IsSuccessStatusCode ? "Skills updated!" : "Failed.";

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> AddSkill(SkillDTO skill)
        {
            string apiUrl = "https://localhost:7192/Employee/AddSkills";

            using (var client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync(apiUrl, skill);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Profile");
                }
                else
                {
                    ViewBag.Error = "Failed to add skill.";
                    return View("Error");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCertificate(CertificateDTO certificate)
        {
            certificate.EmployId = certificate.EmployId == 0 ? Convert.ToInt32(Request.Form["EmployId"]) : certificate.EmployId;

            string apiUrl = "https://localhost:7192/Employee/AddCertificate";

            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync(apiUrl, certificate);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Profile", new { id = certificate.EmployId });
            }

            ViewBag.Error = "Failed to add certificate.";
            return View("Error");
        }



        [HttpPost]
        public async Task<IActionResult> UpdateCertificates(List<CertificateDTO> certificates)
        {
            int? employeeId = HttpContext.Session.GetInt32("employeeId");
            if (employeeId == null)
            {
                TempData["Error"] = "Session expired. Please login again.";
                return RedirectToAction("Login", "Auth");
            }

            var request = new UpdateCertificatesRequest
            {
                EmployeeId = employeeId.Value,
                Certificates = certificates
            };

            using var client = new HttpClient();
            string apiUrl = "https://localhost:7192/Employee/UpdateCertificates";

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(apiUrl, content);

            TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                response.IsSuccessStatusCode ? "Certificates updated!" : "Failed to update certificates.";

            return RedirectToAction("Profile");
        }

    }
}
