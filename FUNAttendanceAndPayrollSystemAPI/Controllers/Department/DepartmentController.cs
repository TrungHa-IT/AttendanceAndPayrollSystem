using DataTransferObject.DepartmentDTO;
using DataTransferObject.EmployeeDTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.DepartmentRepository;

namespace FUNAttendanceAndPayrollSystemAPI.Controllers.Department
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentRepository repository = new DepartmentRepository();
        [HttpGet("getDepartment")]
        public ActionResult<IEnumerable<DepartmentDTO>> GetDepartments() => repository.GetDepartments();

        [HttpPost("createDepartment")]
        public bool CreateDepartment([FromBody] DepartmentDTO department)
        {
            if (department == null || string.IsNullOrEmpty(department.DepartmentName))
            {
                return false; // Invalid input
            }
            return repository.AddDepartment(department);
        }

        [HttpPut("updateDepartment")]
        public bool UpdateDepartment([FromBody] DepartmentDTO department)
        {
            if (department == null || string.IsNullOrEmpty(department.DepartmentName) || department.DepartmentId <= 0)
            {
                return false; // Invalid input
            }
            return repository.UpdateDepartment(department);
        }

        [HttpDelete("deleteDepartment/{id}")]
        public bool DeleteDepartment(int id)
        {
            if (id <= 0)
            {
                return false; // Invalid input
            }
            return repository.DeleteDepartment(id);
        }
    }
}
