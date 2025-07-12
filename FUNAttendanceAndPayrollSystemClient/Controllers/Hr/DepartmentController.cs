using DataTransferObject.DepartmentDTO;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace FUNAttendanceAndPayrollSystemClient.Controllers.Hr
{
    public class DepartmentController : Controller
    {
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public DepartmentController(IConfiguration configuration)
        {
            _baseUrl = configuration["ApiUrls:Base"]!;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // GET: Department List
        public async Task<IActionResult> Department()
        {
            using HttpClient client = new();
            var departmentRes = await client.GetAsync($"{_baseUrl}/api/Department/getDepartment");
            var departmentJson = await departmentRes.Content.ReadAsStringAsync();
            var departments = JsonSerializer.Deserialize<List<DepartmentDTO>>(departmentJson, _jsonOptions);
            return View("~/Views/Hr/Department.cshtml", departments);
        }

        // POST: Create
        [HttpPost]
        public async Task<IActionResult> Create(DepartmentDTO model)
        {
            using HttpClient client = new();
            model.CreatedAt = DateTime.Now;
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_baseUrl}/api/Department/createDepartment", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Department");

            return BadRequest("Error creating department");
        }

        // POST: Edit
        [HttpPost]
        public async Task<IActionResult> Edit(DepartmentDTO model)
        {
            using HttpClient client = new();
            model.UpdatedAt = DateTime.Now;
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_baseUrl}/api/Department/updateDepartment", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Department");

            return BadRequest("Error updating department");
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int id)
        {
            using HttpClient client = new();
            var response = await client.DeleteAsync($"{_baseUrl}/api/Department/deleteDepartment/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Department");

            return BadRequest("Error deleting department");
        }
    }
}
