using DataTransferObject.EmployeeDTOS;
using Microsoft.AspNetCore.Mvc;

namespace FUNAttendanceAndPayrollSystemClient.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterTimeOT(OTRequestDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Reason))
            {
                ViewBag.Message = "Reason is required.";
                return View("Index"); 
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7192");

                var response = await client.PostAsJsonAsync("/Employee/RegisterScheduleOT", model);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    TempData["Success"] = "OT registered successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = "Error: " + error;
                    return RedirectToAction("Index");
                }
            }
        }

    }
}
