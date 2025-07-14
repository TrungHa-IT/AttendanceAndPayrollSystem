using Microsoft.AspNetCore.Mvc;

namespace FUNAttendanceAndPayrollSystemClient.Controllers.Hr
{
    public class HrController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
