using DataTransferObject.DepartmentDTO;
using DataTransferObject.LeaveTypeDTO;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace FUNAttendanceAndPayrollSystemClient.Controllers.Hr
{
    public class LeaveTypeController : Controller
    {
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public LeaveTypeController(IConfiguration configuration)
        {
            _baseUrl = configuration["ApiUrls:Base"]!;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // GET: Department List
        public async Task<IActionResult> LeaveType()
        {
            using HttpClient client = new();
            var leaveTypeRes = await client.GetAsync($"{_baseUrl}/api/LeaveType/getLeaveType");
            var leaveTypeJson = await leaveTypeRes.Content.ReadAsStringAsync();
            var leaveTypes = JsonSerializer.Deserialize<List<LeaveTypeDTO>>(leaveTypeJson, _jsonOptions);
            return View("~/Views/Hr/LeaveType.cshtml", leaveTypes);
        }

        [HttpPost]
        public async Task<IActionResult> Create(LeaveTypeDTO model)
        {
            model.CreatedAt = DateTime.Now;

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            using HttpClient client = new();
            var response = await client.PostAsync($"{_baseUrl}/api/LeaveType/createLeaveType", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("LeaveType");

            return BadRequest("Error creating leaveType");
        }

        //Edit
        [HttpPost]
        public async Task<IActionResult> Edit(LeaveTypeDTO model)
        {
            using HttpClient client = new();
            model.UpdatedAt = DateTime.Now;
            if (model.IsPaid == null)
            {
                model.IsPaid = false;
            }

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_baseUrl}/api/LeaveType/updateLeaveType", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("LeaveType");

            return BadRequest("Error updating leaveType");
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int id)
        {
            using HttpClient client = new();
            var response = await client.DeleteAsync($"{_baseUrl}/api/LeaveType/deleteLeaveType/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction("LeaveType");

            return BadRequest("Error deleting leaveType");
        }
    }
}
