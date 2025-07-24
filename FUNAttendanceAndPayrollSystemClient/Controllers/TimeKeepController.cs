using BusinessObject.Models;
using DataTransferObject.AttendanceDTO;
using DataTransferObject.DateTimeDTO;
using DataTransferObject.ManagerDTO;
using DocumentFormat.OpenXml.InkML;
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
            _apiBaseUrl = $"{configuration["ApiUrls:Base"]}";
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IActionResult> Index(int year = 0, string week = null)
        {
            int? currentEmployeeId = HttpContext.Session.GetInt32("employeeId");
            DateTime todays = DateTime.Today;

            var otResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/ManageSchedule/GetScheduleApproved?employeeId={currentEmployeeId}");

            if (otResponse.IsSuccessStatusCode)
            {
                var otJson = await otResponse.Content.ReadAsStringAsync();
                var approvedOTs = JsonSerializer.Deserialize<List<ApprovedOTDTO>>(otJson, _jsonOptions);
                ViewBag.ApprovedOTs = approvedOTs;
            }

            var statusResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/ManageSchedule/OTStatus?empId={currentEmployeeId}");

            if (statusResponse.IsSuccessStatusCode)
            {
                var statusJson = await statusResponse.Content.ReadAsStringAsync();
                var statusList = JsonSerializer.Deserialize<List<OTStatusDTO>>(statusJson, _jsonOptions);

                ViewBag.OTStatusList = statusList;

            }

            if (year == 0)
                year = DateTime.Now.Year;

            var weekResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/Timekeeping/weeks?year={year}");
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

            var dayResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/Timekeeping/days?year={year}&week={Uri.EscapeDataString(week)}");
            var dayJson = await dayResponse.Content.ReadAsStringAsync();
            var days = JsonSerializer.Deserialize<List<DateTime>>(dayJson, _jsonOptions);

            ViewBag.Year = year;
            ViewBag.WeekOptions = weekOptions;
            ViewBag.SelectedWeek = week;
            ViewBag.DaysInWeek = days;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CheckInOT(int requestId)
        {
            try 
            { 
                string apiUrl = $"{_apiBaseUrl}/ManageSchedule/CheckInOt?requestId={requestId}";

                var response = await _httpClient.PostAsync(apiUrl, null); 

                if (!response.IsSuccessStatusCode)
                {
                    var errorDetail = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { message = "Check In OT failed", detail = errorDetail });
                }

                var json = await response.Content.ReadAsStringAsync();
                return Json(new { success = true, data = json });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Client error", detail = ex.Message });
            }
        }
        //https://localhost:7192/ManageSchedule/CheckOutOt?requestId=2
        [HttpPost]
        public async Task<IActionResult> CheckOutOT(int requestId)
        {
            try
            {
                string apiUrl = $"{_apiBaseUrl}/ManageSchedule/CheckOutOt?requestId={requestId}";

                var response = await _httpClient.PostAsync(apiUrl, null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorDetail = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { message = "Check In OT failed", detail = errorDetail });
                }

                var json = await response.Content.ReadAsStringAsync();
                return Json(new { success = true, data = json });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Client error", detail = ex.Message });
            }
        }

    }
}
