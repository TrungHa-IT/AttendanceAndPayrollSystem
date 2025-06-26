using DataTransferObject.DateTimeDTO;
using FUNAttendanceAndPayrollSystemClient.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FUNAttendanceAndPayrollSystemClient.Controllers
{
    public class TimeKeepController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public TimeKeepController(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = $"{configuration["ApiUrls:Base"]}/Timekeeping";
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IActionResult> Index(int year = 0, string week = null)
        {
            if (year == 0)
                year = DateTime.Now.Year;

           
            var weekResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/weeks?year={year}");
            if (!weekResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không thể tải danh sách tuần.";
                return View();
            }

            var weeksJson = await weekResponse.Content.ReadAsStringAsync();
            var weekOptions = JsonSerializer.Deserialize<List<WeekOption>>(weeksJson, _jsonOptions);

           
            if (string.IsNullOrEmpty(week))
            {
                var today = DateTime.Today;
                var currentWeek = weekOptions?.FirstOrDefault(w => today >= w.StartDate && today <= w.EndDate);
                week = currentWeek?.Display;
            }

           
            var dayResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/days?year={year}&week={Uri.EscapeDataString(week)}");
            var dayJson = await dayResponse.Content.ReadAsStringAsync();
            var days = JsonSerializer.Deserialize<List<DateTime>>(dayJson, _jsonOptions);

            ViewBag.Year = year;
            ViewBag.WeekOptions = weekOptions;
            ViewBag.SelectedWeek = week;
            ViewBag.DaysInWeek = days;

            return View();
        }
    }
}
