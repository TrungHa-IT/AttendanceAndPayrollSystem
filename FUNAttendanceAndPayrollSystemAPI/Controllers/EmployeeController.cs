using BusinessObject.Models;
using DataAccess.EmployeeDAO;
using Microsoft.AspNetCore.Mvc;
using Repository.EmployeeRepository;
using System.Collections;

namespace FUNAttendanceAndPayrollSystemAPI.Controllers
{
    [Route("Employee/")]
    [ApiController]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository repository = new EmployeeRepository();
        [HttpGet("ListEmployees")]
        public ActionResult<IEnumerable<Employee>> GetEmployees() => repository.GetEmployees();
    }
}
