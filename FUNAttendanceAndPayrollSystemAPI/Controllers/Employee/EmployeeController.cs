using BusinessObject.Models;
using DataAccess.EmployeeDAO;
using DataTransferObject.EmployeeDTO;
using DataTransferObject.EmployeeDTOS;
using Microsoft.AspNetCore.Mvc;
using Repository.EmployeeRepository;
using System.Collections;

namespace FUNAttendanceAndPayrollSystemAPI.Controllers.Employee
{
    [Route("Employee/")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository repository = new EmployeeRepository();
        [HttpGet("ListEmployees")]
        public ActionResult<IEnumerable<EmployeeDTO>> GetEmployees() => repository.GetEmployees();

        [HttpGet("ListStaff")]
        public ActionResult<IEnumerable<EmployeeDTO>> GetStaffs() => repository.GetStaffs();

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

        [HttpPost("RegisterScheduleOT")]
        public IActionResult RegisterScheduleOT([FromBody] OTRequestDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Reason))
            {
                return BadRequest("Reason must not be empty.");
            }

            try
            {
                var empAttendance = repository.bookScheduleOverTime(model.Emp,
            model.OtDate,
            model.StartTime,
            model.EndTime,
            model.Reason);

                if (empAttendance == null)
                {
                    return NotFound("Employee not found or unable to register OT.");
                }

                return Ok(empAttendance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("getTotalHourByEmployee")]
        public ActionResult<IEnumerable<EmployeeDTO>> getTotalHourByEmployee(int? month, int? year, int? departmenId) => repository.GetEmployeesTotalTimeByMonth(month,year,departmenId);


    }
}
