using DataAccess.ManagerDAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.ManagerRepository;

namespace FUNAttendanceAndPayrollSystemAPI.Controllers.Manager
{
    [Route("[controller]/")]
    public class CaculateSalaryController : ControllerBase
    {
        private readonly IManageSalaryRepository repository = new ManageSalaryRepository();
        [HttpPost("CalculateSalary")]
        public IActionResult CalculateSalary(int employeeId, int month, int year)
        {
            var result = repository.CaculateSalary(employeeId, month, year);

            if (result.Contains("not found"))
                return NotFound(result);

            return Ok(new { message = result });
        }
    }

}
