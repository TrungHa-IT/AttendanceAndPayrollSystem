using DataTransferObject.AuthDTO;
using DataTransferObject.DepartmentDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FUNAttendanceAndPayrollSystemClient.Controllers
{
    public class AuthController : Controller
    {
        private readonly string _loginUrl;
        private readonly string _registerUrl;
        private readonly string _baseUrl;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonOptions;

        public AuthController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            var baseUrl = configuration["ApiUrls:Base"]; // ví dụ: "https://localhost:7192"
            _loginUrl = $"{baseUrl}/api/Auth/login";
            _registerUrl = $"{baseUrl}/api/Auth/register";
            _baseUrl = baseUrl;
            _httpClientFactory = httpClientFactory;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_loginUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Invalid email or password!";
                return View();
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(json).RootElement;

            var token = result.GetProperty("token").GetString() ?? "";
            var name = result.GetProperty("name").GetString() ?? "";
            var role = result.GetProperty("role").GetString() ?? "";
            var email = result.GetProperty("email").GetString() ?? "";
            var employeeId = result.TryGetProperty("employeeId", out var empIdProp) ? empIdProp.GetInt32() : 0;

            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("name", name);
            HttpContext.Session.SetString("email", email);
            HttpContext.Session.SetString("role", role);
            HttpContext.Session.SetInt32("employeeId", employeeId);

            return role.ToLower() switch
            {
                "employee" => RedirectToAction("Dashboard", "Employee"),
                "staff" => RedirectToAction("Dashboard", "Staff"),
                "hr" => RedirectToAction("Dashboard", "Hr"),
                _ => (TempData["Error"] = "Unknown role.") == null ? View() : View()
            };
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.GetAsync($"{_baseUrl}/api/Department/getDepartment");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Failed to fetch departments from server.";
                    ViewBag.Departments = new List<DepartmentDTO>();
                    return View();
                }

                var departmentJson = await response.Content.ReadAsStringAsync();
                var departments = JsonSerializer.Deserialize<List<DepartmentDTO>>(departmentJson, _jsonOptions);
                ViewBag.Departments = departments ?? new List<DepartmentDTO>();
                return View();
            }
            catch
            {
                TempData["Error"] = "An error occurred while loading departments.";
                ViewBag.Departments = new List<DepartmentDTO>();
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(IFormCollection form, IFormFile Image)
        {
            var client = _httpClientFactory.CreateClient();
            var content = new MultipartFormDataContent();

            // Add form fields
            foreach (var key in form.Keys)
            {
                content.Add(new StringContent(form[key]), key);
            }

            // Add file
            if (Image != null && Image.Length > 0)
            {
                var streamContent = new StreamContent(Image.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(Image.ContentType);
                content.Add(streamContent, "Image", Image.FileName);
            }

            var response = await client.PostAsync(_registerUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Registered successfully!";
                return RedirectToAction("Login");
            }

            var depRes = await client.GetAsync($"{_baseUrl}/api/Department/getDepartment");
            if (depRes.IsSuccessStatusCode)
            {
                var depJson = await depRes.Content.ReadAsStringAsync();
                var departments = JsonSerializer.Deserialize<List<DepartmentDTO>>(depJson, _jsonOptions);
                ViewBag.Departments = departments ?? new List<DepartmentDTO>();
            }

            TempData["Error"] = "Failed to register. Email may already exist.";
            return View();
        }
    }
}
