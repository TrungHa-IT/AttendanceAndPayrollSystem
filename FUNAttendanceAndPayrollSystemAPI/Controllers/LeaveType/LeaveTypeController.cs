using DataTransferObject.DepartmentDTO;
using DataTransferObject.EmployeeDTOS;
using DataTransferObject.LeaveTypeDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.LeaveTypeRepository;

namespace FUNAttendanceAndPayrollSystemAPI.Controllers.LeaveType
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveTypeController : ControllerBase
    {
        private readonly ILeaveTypeRepository repository = new LeaveTypeRepository();
        [HttpGet("getLeaveType")]
        public ActionResult<IEnumerable<LeaveTypeDTO>> GetLeaveType() => repository.GetLeaveTypes();

        [HttpPut("updateLeaveType")]
        public bool UpdateLeaveType([FromBody] LeaveTypeDTO leaveType)
        {
            if (leaveType == null || leaveType.LeaveTypeId <= 0)
            {
                return false; // Invalid input
            }
            return repository.UpdateLeaveTypes(leaveType);
        }

        [HttpDelete("deleteLeaveType/{id}")]
        public IActionResult DeleteLeaveType(int id)
        {
            var leaveType = repository.GetLeaveTypes()
                                      .FirstOrDefault(l => l.LeaveTypeId == id);

            if (leaveType == null)
            {
                return NotFound(new
                {
                    Message = $"Leave type with ID {id} not found."
                });
            }

            repository.DeleteLeaveTypes(id);

            return Ok(new
            {
                Message = $"Leave type with ID {id} has been deleted successfully."
            });
        }


        [HttpPost("createLeaveType")]
        public ActionResult<LeaveTypeDTO> CreateLeaveType(LeaveTypeDTO leaveType)
        {
            if (leaveType == null)
            {
                return BadRequest("Leave type cannot be null.");
            }
            var createdLeaveType = repository.AddLeaveTypes(leaveType);
            return Ok(createdLeaveType);
        }
    }
}
