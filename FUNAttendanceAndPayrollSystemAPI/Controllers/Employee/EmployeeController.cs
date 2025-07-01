using BusinessObject.Models;
using DataAccess.EmployeeDAO;
using DataTransferObject.EmployeeDTO;
using Microsoft.AspNetCore.Mvc;
using Repository.EmployeeRepository;
using System.Collections;

namespace FUNAttendanceAndPayrollSystemAPI.Controllers.Employee
{
    [Route("Employee/")]
    [ApiController]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository repository = new EmployeeRepository();
        [HttpGet("ListEmployees")]
        public ActionResult<IEnumerable<EmployeeDTO>> GetEmployees() => repository.GetEmployees();

        [HttpGet("MyAttendance")]
        public IActionResult GetAttendanceById(int emp)
        {
            var empAttendance = repository.getAttendanceById(emp);
            if (empAttendance == null)
            {
                return BadRequest("emp id values must not be null.");
            }
            return Ok(empAttendance);
        }
    }
}
