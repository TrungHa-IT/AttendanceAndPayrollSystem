using DataTransferObject.DepartmentDTO;
using DataTransferObject.EmployeeDTOS;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace FUNAttendanceAndPayrollSystemClient.Controllers.Hr
{
    public class EmployeeController : Controller
    {
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public EmployeeController(IConfiguration configuration)
        {
            _baseUrl = configuration["ApiUrls:Base"]!;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public async Task<IActionResult> Employee()
        {
            using HttpClient client = new(); 
            var response = await client.GetAsync($"{_baseUrl}/api/Department/getDepartment");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var departments = JsonSerializer.Deserialize<List<DepartmentDTO>>(json, _jsonOptions);
                ViewBag.Departments = departments ?? new List<DepartmentDTO>();
            }
            else
            {
                ViewBag.Departments = new List<DepartmentDTO>();
            }
            var employeeRes = await client.GetAsync($"{_baseUrl}/Employee/ListEmployees");
            var employeeJson = await employeeRes.Content.ReadAsStringAsync();
            var employees = JsonSerializer.Deserialize<List<EmployeeDTO>>(employeeJson, _jsonOptions);

     
            return View("~/Views/Hr/Employee.cshtml", employees);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            using HttpClient client = new();

            var employeeResponse = await client.GetAsync($"{_baseUrl}/Employee/{id}");
            if (!employeeResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Employee not found";
                return RedirectToAction("Employee");
            }

            var employeeJson = await employeeResponse.Content.ReadAsStringAsync();
            var employee = JsonSerializer.Deserialize<EmployeeUpdateDTO>(employeeJson, _jsonOptions);

            // Load departments
            var deptResponse = await client.GetAsync($"{_baseUrl}/api/Department/getDepartment");
            if (deptResponse.IsSuccessStatusCode)
            {
                var deptJson = await deptResponse.Content.ReadAsStringAsync();
                var departments = JsonSerializer.Deserialize<List<DepartmentDTO>>(deptJson, _jsonOptions);
                ViewBag.Departments = departments ?? new List<DepartmentDTO>();
            }
            else
            {
                ViewBag.Departments = new List<DepartmentDTO>();
                TempData["Error"] = "Failed to load departments";
            }

            return View("~/Views/Hr/Employee.cshtml", employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeUpdateDTO model)
        {
            using HttpClient client = new();

            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_baseUrl}/Employee/UpdateEmployee/{model.EmployId}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Update successful.";
                return RedirectToAction("Employee");
            }

            TempData["ErrorMessage"] = "Update failed.";

         
            var deptResponse = await client.GetAsync($"{_baseUrl}/api/Department/getDepartment");
            if (deptResponse.IsSuccessStatusCode)
            {
                var deptJson = await deptResponse.Content.ReadAsStringAsync();
                var departments = JsonSerializer.Deserialize<List<DepartmentDTO>>(deptJson, _jsonOptions);
                ViewBag.Departments = departments ?? new List<DepartmentDTO>();
            }
            else
            {
                ViewBag.Departments = new List<DepartmentDTO>();
            }

            var employeeRes = await client.GetAsync($"{_baseUrl}/Employee/ListEmployees");
            var employeeJson = await employeeRes.Content.ReadAsStringAsync();
            var employees = JsonSerializer.Deserialize<List<EmployeeDTO>>(employeeJson, _jsonOptions);

            return View("~/Views/Hr/Employee.cshtml", employees);
        }



        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            using HttpClient client = new();
            try
            {
                var response = await client.DeleteAsync($"{_baseUrl}/Employee/DeleteEmployee/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Employee deleted successfully";
                    return RedirectToAction("Employee");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Error deleting employee: {errorContent}";
                return RedirectToAction("Employee");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Network error: {ex.Message}";
                return RedirectToAction("Employee");
            }
        }
    }
}
