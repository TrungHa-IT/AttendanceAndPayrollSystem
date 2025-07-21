using DataTransferObject.EmployeeDTOS;
using Microsoft.AspNetCore.Mvc;

namespace FUNAttendanceAndPayrollSystemClient.Controllers.Employee
{
    public class EmployeeController : Controller
    {
        public IActionResult Dashboard()
        {
            int? employeeId = HttpContext.Session.GetInt32("employeeId");
            ViewBag.EmployeeId = employeeId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterTimeOT(OTRequestDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Reason))
            {
                TempData["Error"] = "Reason is required.";
                return RedirectToAction("Dashboard");
            }

            var today = DateOnly.FromDateTime(DateTime.Now);
            if (model.OtDate <= today)
            {
                TempData["Error"] = "Please select a date after today.";
                return RedirectToAction("Dashboard");
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7192");

                var response = await client.PostAsJsonAsync("/Employee/RegisterScheduleOT", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "OT registered successfully.";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = "Error: " + error;
                    return RedirectToAction("Dashboard");
                }
            }
        }

    }
}
