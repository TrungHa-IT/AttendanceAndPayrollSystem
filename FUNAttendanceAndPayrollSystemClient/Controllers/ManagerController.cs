using Microsoft.AspNetCore.Mvc;

namespace FUNAttendanceAndPayrollSystemClient.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
