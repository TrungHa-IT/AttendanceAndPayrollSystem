using Microsoft.AspNetCore.Mvc;

namespace FUNAttendanceAndPayrollSystemClient.Controllers.Employee
{
    public class EmployeeController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
