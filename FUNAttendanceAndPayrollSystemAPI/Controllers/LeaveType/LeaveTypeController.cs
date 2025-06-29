using DataTransferObject.EmployeeDTO;
using DataTransferObject.LeaveTypeDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.EmployeeRepository;
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

        [HttpGet("updateLeaveType")]
        public ActionResult<LeaveTypeDTO> UpdateLeaveType(int id, LeaveTypeDTO leaveType)
        {
            if (id != leaveType.LeaveTypeId)
            {
                return BadRequest("Leave type ID mismatch.");
            }
            var updatedLeaveType = repository.UpdateLeaveTypes(leaveType);
            if (updatedLeaveType == null)
            {
                return NotFound("Leave type not found.");
            }
            return Ok(updatedLeaveType);
        }

        [HttpGet("deleteLeaveType")]
        public ActionResult DeleteLeaveType(int id)
        {
            var leaveType = repository.GetLeaveTypes().FirstOrDefault(l => l.LeaveTypeId == id);
            if (leaveType == null)
            {
                return NotFound("Leave type not found.");
            }
            repository.DeleteLeaveTypes(id);
            return NoContent();
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
