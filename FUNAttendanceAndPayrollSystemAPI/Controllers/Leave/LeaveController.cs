using DataTransferObject.LeaveDTO;
using DataTransferObject.LeaveTypeDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.LeaveRepository;
using Repository.LeaveTypeRepository;

namespace FUNAttendanceAndPayrollSystemAPI.Controllers.Leaves
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveRepository repository = new LeaveRepository();
        [HttpGet("getLeave")]
        public ActionResult<IEnumerable<LeaveDTO>> GetLeaveType() => repository.GetLeaves();

        [HttpGet("updateLeave")]
        public ActionResult<LeaveDTO> UpdateLeave(int id, LeaveDTO leave)
        {
            if (id != leave.LeaveTypeId)
            {
                return BadRequest("Leave ID mismatch.");
            }
            var updatedLeave = repository.UpdateLeave(leave);
            if (updatedLeave == null)
            {
                return NotFound("Leave type not found.");
            }
            return Ok(updatedLeave);
        }

        [HttpGet("deleteLeave")]
        public ActionResult DeleteLeave(int id)
        {
            var leaveType = repository.GetLeaves().FirstOrDefault(l => l.LeaveId == id);
            if (leaveType == null)
            {
                return NotFound("Leave type not found.");
            }
            repository.DeleteLeave(id);
            return NoContent();
        }

        [HttpPost("createLeave")]
        public ActionResult<LeaveTypeDTO> CreateLeave(LeaveDTO leaveDTO)
        {
            if (leaveDTO == null)
            {
                return BadRequest("Leave cannot be null.");
            }
            var createdLeaveType = repository.AddLeave(leaveDTO);
            return Ok(CreateLeave);
        }
    }
}
