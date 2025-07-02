using DataTransferObject.AuthDTO;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using DataTransferObject.DepartmentDTO;
using Microsoft.AspNetCore.Http;

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
            var baseUrl = configuration["ApiUrls:Base"]; // "https://localhost:7192"
            _loginUrl = $"{baseUrl}/api/Auth/login";
            _registerUrl = $"{baseUrl}/api/Auth/register";
            _baseUrl = baseUrl;
            _httpClientFactory = httpClientFactory;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
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
            var employeeId = result.TryGetProperty("employeeId", out var empIdProp) ? empIdProp.GetInt32() : 0;

            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("name", name);
            HttpContext.Session.SetString("role", role);
            HttpContext.Session.SetInt32("employeeId", employeeId);

            switch (role.ToLower())
            {
                case "employee":
                    return RedirectToAction("Dashboard", "Employee");
                case "staff":
                    return RedirectToAction("Dashboard", "Staff", new { area = "Staff" });
                case "hr":
                    return RedirectToAction("Dashboard", "Hr", new { area = "Hr" });
                default:
                    TempData["Error"] = "Unknown role.";
                    return View();
            }
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
            catch (Exception ex)
            {
                // Log exception if needed
                TempData["Error"] = "An error occurred while loading departments.";
                ViewBag.Departments = new List<DepartmentDTO>();
                return View();
            }
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var client = _httpClientFactory.CreateClient();

            var content = new StringContent(JsonSerializer.Serialize(registerDTO), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_registerUrl, content);

            if (!response.IsSuccessStatusCode)
            {

                // 👉 Đọc nội dung phản hồi lỗi chi tiết
                var errorJson = await response.Content.ReadAsStringAsync();

                var departmentRes = await client.GetAsync($"{_baseUrl}/api/Department/getDepartment");
                if (departmentRes.IsSuccessStatusCode)
                {
                    var departmentJson = await departmentRes.Content.ReadAsStringAsync();
                    var departments = JsonSerializer.Deserialize<List<DepartmentDTO>>(departmentJson, _jsonOptions);
                    ViewBag.Departments = departments ?? new List<DepartmentDTO>();
                }
                else
                {
                    ViewBag.Departments = new List<DepartmentDTO>();
                }

                TempData["Error"] = "Email already exists!";
                return View(registerDTO);
            }

            TempData["Success"] = "Registered successfully!";
            return RedirectToAction("Login");
        }

    }
}
