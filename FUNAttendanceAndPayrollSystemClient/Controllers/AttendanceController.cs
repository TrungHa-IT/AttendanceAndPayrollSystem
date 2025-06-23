using Microsoft.AspNetCore.Mvc;

namespace FUNAttendanceAndPayrollSystemClient.Controllers
{
    public class AttendanceController : Controller
    {
        public IActionResult ManageAttendance()
        {
            return View();
        }
    }
}
