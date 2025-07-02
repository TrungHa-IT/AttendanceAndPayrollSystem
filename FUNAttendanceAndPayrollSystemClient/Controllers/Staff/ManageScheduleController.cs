using DataTransferObject.EmployeeDTO;
using DataTransferObject.EmployeeDTOS;
using DataTransferObject.ManagerDTO;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace FUNAttendanceAndPayrollSystemClient.Controllers.Staff
{
    public class ManageScheduleController : Controller
    {
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public ManageScheduleController(IConfiguration configuration)
        {
            _baseUrl = configuration["ApiUrls:Base"]!;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IActionResult> Index()
        {
            int? id = HttpContext.Session.GetInt32("employeeId");
            using HttpClient client = new();
            var employeeRes = await client.GetAsync($"{_baseUrl}/Employee/ListEmployees");
            var employeeJson = await employeeRes.Content.ReadAsStringAsync();
            var employees = JsonSerializer.Deserialize<List<EmployeeDTO>>(employeeJson, _jsonOptions);
            ViewBag.Employees = employees;

            if (id.HasValue)
            {
                var scheduleRes = await client.GetAsync($"{_baseUrl}/ManageSchedule/GetScheduleByEmployee/{id}");
                if (scheduleRes.IsSuccessStatusCode)
                {
                    var scheduleJson = await scheduleRes.Content.ReadAsStringAsync();
                    var schedules = JsonSerializer.Deserialize<List<ScheduleDTO>>(scheduleJson, _jsonOptions);
                    ViewBag.Schedules = schedules;
                }
                else ViewBag.Schedules = new List<ScheduleDTO>();
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSchedulesByEmployee(int employeeId)
        {
            using HttpClient client = new();
            var res = await client.GetAsync($"{_baseUrl}/ManageSchedule/GetScheduleByEmployee/{employeeId}");
            if (!res.IsSuccessStatusCode) return Json(new List<ScheduleDTO>());

            var json = await res.Content.ReadAsStringAsync();
            var schedules = JsonSerializer.Deserialize<List<ScheduleDTO>>(json, _jsonOptions);
            return Json(schedules);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] ScheduleRequestDTO request)
        {
            if (request.StartDate < DateOnly.FromDateTime(DateTime.Today))
                return BadRequest(new { message = "Start date cannot be in the past" });

            if (request.EndDate < request.StartDate)
                return BadRequest(new { message = "End date cannot be before start date" });

            using HttpClient client = new();
            var res = await client.PostAsJsonAsync($"{_baseUrl}/ManageSchedule/CreateScheduleEmployee", request);
            var content = await res.Content.ReadAsStringAsync();

            if (res.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<CreateScheduleResultDTO>(content, _jsonOptions);
                return Ok(new { message = "Schedule created", duplicated = result?.Duplicated });
            }

            return BadRequest(new { message = "Failed to create schedule" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSchedule([FromBody] ScheduleRequestDTO request)
        {
            if (request.StartDate < DateOnly.FromDateTime(DateTime.Today))
                return BadRequest(new { message = "Start date cannot be in the past" });

            if (request.EndDate < request.StartDate)
                return BadRequest(new { message = "End date cannot be before start date" });

            try
            {
                using HttpClient client = new();
                var query = $"?employeeId={request.EmployeeId}&startDate={request.StartDate:yyyy-MM-dd}&endDate={request.EndDate:yyyy-MM-dd}";
                var res = await client.DeleteAsync($"{_baseUrl}/ManageSchedule/DeleteScheduleEmployee{query}");

                if (res.IsSuccessStatusCode)
                    return Ok(new { message = "Schedule deleted successfully!" });

                var error = await res.Content.ReadAsStringAsync();
                return StatusCode((int)res.StatusCode, new { message = "Failed to delete schedule.", details = error });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> generateWeek([FromBody] DateOnly date)
        {
            using HttpClient client = new();
            var dt = date.ToDateTime(TimeOnly.MinValue);
            var json = new StringContent(JsonSerializer.Serialize(dt), Encoding.UTF8, "application/json");

            var res = await client.PostAsync($"{_baseUrl}/ManageSchedule/generate-week-schedule", json);
            var raw = await res.Content.ReadAsStringAsync();

            if (res.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<GenerateScheduleResponse>(raw, _jsonOptions);
                TempData["Message"] = result?.Message;
                if (result?.Skipped != null && result.Skipped.Any())
                    ViewBag.SkippedSchedules = result.Skipped;
            }
            else TempData["Error"] = $"Failed to generate schedule. Status: {res.StatusCode}. Details: {raw}";

            var empRes = await client.GetAsync($"{_baseUrl}/Employee/ListEmployees");
            var empJson = await empRes.Content.ReadAsStringAsync();
            var employees = JsonSerializer.Deserialize<List<EmployeeDTO>>(empJson, _jsonOptions);
            ViewBag.Employees = employees;

            return View("Index");
        }

        public async Task<IActionResult> ManageOverTime()
        {
            HttpClient client = new HttpClient();
            string uri = $"{_baseUrl}/ManageSchedule/ManageOverTime";

            HttpResponseMessage response = await client.GetAsync(uri);
            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            List<BookingOTDTO> schedules = new();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                schedules = JsonSerializer.Deserialize<List<BookingOTDTO>>(json, option);
            }

            return View(schedules);
        }


        [HttpGet]
        public async Task<IActionResult> CheckInStatus()
        {
            int? empId = HttpContext.Session.GetInt32("employeeId");
            if (empId == null)
                return Unauthorized();

            using HttpClient client = new();
            var res = await client.GetAsync($"{_baseUrl}/ManageSchedule/checkInStatus?empId={empId}");

            if (!res.IsSuccessStatusCode)
                return BadRequest(new { hasCheckedIn = false, message = "API failed" });

            var json = await res.Content.ReadAsStringAsync();
            var root = JsonDocument.Parse(json).RootElement;
            return Ok(new { hasCheckedIn = root.GetProperty("hasCheckedIn").GetBoolean() });
        }


        [HttpPost]
        public async Task<IActionResult> CheckIn()
        {
            int? empId = HttpContext.Session.GetInt32("employeeId");
            var content = new StringContent(JsonSerializer.Serialize(empId), Encoding.UTF8, "application/json");
            using HttpClient client = new();
            var res = await client.PostAsync($"{_baseUrl}/ManageSchedule/checkIn", content);
            var json = await res.Content.ReadAsStringAsync();

            if (res.IsSuccessStatusCode)
                return Ok(new { message = "Forwarded check-in successful", json });

            try
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                var msg = dict?.GetValueOrDefault("message") ?? "Unknown error";
                return StatusCode((int)res.StatusCode, new { message = msg });
            }
            catch
            {
                return StatusCode((int)res.StatusCode, new { message = "Check-in failed" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckOut()
        {
            int? empId = HttpContext.Session.GetInt32("employeeId");
            var content = new StringContent(JsonSerializer.Serialize(empId), Encoding.UTF8, "application/json");
            using HttpClient client = new();
            var res = await client.PostAsync($"{_baseUrl}/ManageSchedule/checkOut", content);
            var json = await res.Content.ReadAsStringAsync();

            if (res.IsSuccessStatusCode)
                return Ok(new { message = "Forwarded check-out successful", json });

            return StatusCode((int)res.StatusCode, new { message = "Forwarded check-out failed", json });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(OTUpdateRequestDTO oTRequest)
        {
            int? managerId = HttpContext.Session.GetInt32("employeeId");
            if (managerId == null)
            {
                TempData["Error"] = "Session expired.";
                return RedirectToAction("ManageOverTime");
            }

            oTRequest.EmpId = managerId.Value;

            using HttpClient client = new();
            var jsonContent = JsonSerializer.Serialize(oTRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            string url = $"{_baseUrl}/ManageSchedule/UpdateBooking";
            var response = await client.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Update Over time successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to update status.";
            }

            return RedirectToAction("ManageOverTime");
        }



    }
}
