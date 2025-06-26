using DataTransferObject.AuthDTO;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace FUNAttendanceAndPayrollSystemClient.Controllers
{
    public class AuthController : Controller
    {
        private readonly string _loginUrl;
        private readonly string _registerUrl;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            var baseUrl = configuration["ApiUrls:Base"]; // "https://localhost:7192"
            _loginUrl = $"{baseUrl}/api/Auth/login";
            _registerUrl = $"{baseUrl}/api/Auth/register";
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var loginDTO = new LoginDTO { Email = email, Password = password };
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

            HttpContext.Session.SetString("token", result.GetProperty("Token").GetString() ?? "");
            HttpContext.Session.SetString("name", result.GetProperty("Name").GetString() ?? "");
            HttpContext.Session.SetString("role", result.GetProperty("Role").GetString() ?? "");

            return RedirectToAction("ManageAttendance", "Attendance");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonSerializer.Serialize(registerDTO), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_registerUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Email already exists!";
                return View(registerDTO);
            }

            TempData["Success"] = "Registered successfully! You can now log in.";
            return RedirectToAction("Login");
        }
    }
}
