using BusinessObject.Models;
using DataTransferObject.EmployeeDTOS;
using Microsoft.AspNetCore.Mvc;

namespace FUNAttendanceAndPayrollSystemClient.Controllers.Hr
{
    public class EmployeeStatisticController : Controller
    {
        private readonly FunattendanceAndPayrollSystemContext _context;

        public EmployeeStatisticController(FunattendanceAndPayrollSystemContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var employees = _context.Employees.ToList();

            var statistics = new EmployeeStatisticDTO
            {
                TotalEmployees = employees.Count,
                ActiveEmployees = employees.Count(e => e.Status == 1),
                InactiveEmployees = employees.Count(e => e.Status == 0),
                MaleEmployees = employees.Count(e => e.Gender != null && e.Gender.ToLower().Contains("male")),
                FemaleEmployees = employees.Count(e => e.Gender != null && e.Gender.ToLower().Contains("female")),
                TotalDepartments = employees.Select(e => e.DepartmentId).Distinct().Count()
            };

            return View("~/Views/Hr/EmployeeStatistic.cshtml", statistics);
        }
    }
}
