using DataTransferObject.EmployeeDTO;
using DataTransferObject.ManagerDTO;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace FUNAttendanceAndPayrollSystemClient.Controllers
{
    public class ManageScheduleController : Controller
    {
        public async Task<IActionResult> Index(int? id)
        {
            HttpClient client = new HttpClient();
            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string url = "https://localhost:7192/Employee/ListEmployees";
            HttpResponseMessage response = await client.GetAsync(url);
            var strData = await response.Content.ReadAsStringAsync();
            List<EmployeeDTO> employees = JsonSerializer.Deserialize<List<EmployeeDTO>>(strData, option);
            ViewBag.Employees = employees;
            if (id.HasValue)
            {
                string urlListSchedule = $"https://localhost:7192/ManageSchedule/GetScheduleByEmployee/{id}";
                HttpResponseMessage scheduleResponse = await client.GetAsync(urlListSchedule);

                if (scheduleResponse.IsSuccessStatusCode)
                {
                    var scheduleData = await scheduleResponse.Content.ReadAsStringAsync();
                    List<ScheduleDTO> schedules = JsonSerializer.Deserialize<List<ScheduleDTO>>(scheduleData, option);
                    ViewBag.Schedules = schedules;
                }
                else
                {
                    ViewBag.Schedules = new List<ScheduleDTO>();
                }
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSchedulesByEmployee(int employeeId)
        {
            string url = $"https://localhost:7192/ManageSchedule/GetScheduleByEmployee/{employeeId}";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var schedules = JsonSerializer.Deserialize<List<ScheduleDTO>>(json, option);
                return Json(schedules);
            }

            return Json(new List<ScheduleDTO>());
        }



        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] ScheduleRequestDTO request)
        {
            if (request.StartDate < DateOnly.FromDateTime(DateTime.Today))
                return BadRequest(new { message = "Start date cannot be in the past" });

            if (request.EndDate < request.StartDate)
                return BadRequest(new { message = "End date cannot be before start date" });

            HttpClient client = new HttpClient();
            var response = await client.PostAsJsonAsync("https://localhost:7192/ManageSchedule/CreateScheduleEmployee", request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<CreateScheduleResultDTO>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return Ok(new
                {
                    message = "Schedule created",
                    duplicated = result?.Duplicated
                });
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
                using var client = new HttpClient();

                var query = $"?employeeId={request.EmployeeId}&startDate={request.StartDate:yyyy-MM-dd}&endDate={request.EndDate:yyyy-MM-dd}";
                var url = $"https://localhost:7192/ManageSchedule/DeleteScheduleEmployee{query}";

                var response = await client.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Schedule deleted successfully!" });
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { message = "Failed to delete schedule.", details = errorContent });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> generateWeek([FromBody] DateOnly date)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7192/");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var dateTime = date.ToDateTime(TimeOnly.MinValue);
            var content = new StringContent(JsonSerializer.Serialize(dateTime), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("ManageSchedule/generate-week-schedule", content);
            var rawResponse = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<GenerateScheduleResponse>(rawResponse, options);
                TempData["Message"] = result.Message;

                if (result.Skipped != null && result.Skipped.Any())
                {
                    ViewBag.SkippedSchedules = result.Skipped;
                }
            }
            else
            {
                TempData["Error"] = $"Failed to generate schedule. Status: {response.StatusCode}. Details: {rawResponse}";
            }

            var employeeResponse = await client.GetAsync("Employee/ListEmployees");
            var employeeJson = await employeeResponse.Content.ReadAsStringAsync();
            var employees = JsonSerializer.Deserialize<List<EmployeeDTO>>(employeeJson, options);
            ViewBag.Employees = employees;

            return View("Index");
        }

        [HttpGet]
        public async Task<IActionResult> CheckInStatus()
        {
            using HttpClient client = new HttpClient();
            var url = "https://localhost:7192/ManageSchedule/checkInStatus";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new { hasCheckedIn = false, message = "API failed" });
            }

            var resultString = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(resultString);
            var root = jsonDoc.RootElement;
            return Ok(new
            {
                hasCheckedIn = root.GetProperty("hasCheckedIn").GetBoolean()
            });
        }


        [HttpPost]
        public async Task<IActionResult> CheckIn()
        {
            using HttpClient client = new HttpClient();

            var url = "https://localhost:7192/ManageSchedule/checkIn";
            var response = await client.PostAsync(url, null);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { message = "Forwarded check-in successful", result });
            }
            else
            {
                try
                {
                    var content = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
                    var backendMessage = content.ContainsKey("message") ? content["message"] : "Unknown error";

                    return StatusCode((int)response.StatusCode, new { message = backendMessage });
                }
                catch
                {
                    return StatusCode((int)response.StatusCode, new { message = "Check-in failed" });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckOut()
        {
            using HttpClient client = new HttpClient();
            var url = "https://localhost:7192/ManageSchedule/checkOut";

            var response = await client.PostAsync(url, null);

            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { message = "Forwarded check-out successful", result });
            }
            else
            {
                return StatusCode((int)response.StatusCode, new { message = "Forwarded check-out failed", result });
            }
        }



    }
}
