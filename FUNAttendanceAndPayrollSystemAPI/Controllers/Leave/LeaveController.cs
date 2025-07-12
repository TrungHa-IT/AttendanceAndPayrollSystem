using DataTransferObject.LeaveDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.LeaveRepository;

namespace FUNAttendanceAndPayrollSystemAPI.Controllers.Leaves
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveRepository repository = new LeaveRepository();

        [HttpGet("getLeave")]
        public ActionResult<IEnumerable<LeaveDTO>> GetLeaves()
        {
            var leaves = repository.GetLeaves();
            return Ok(leaves);
        }

        [HttpGet("getLeaveEmployee")]
        public ActionResult<IEnumerable<LeaveDTO>> GetLeaveEmployee(int id)
        {
            var leaves = repository.GetLeaveEmployee(id);
            return Ok(leaves);
        }

        [HttpGet("getLeaveStaff")]
        public ActionResult<IEnumerable<LeaveDTO>> GetLeaveStaff(int id)
        {
            var leaves = repository.GetLeaveStaff(id);
            return Ok(leaves);
        }

        [HttpPost("createLeave")]
        public bool CreateLeave([FromBody] LeaveDTO leaveDTO)
        {
            if (leaveDTO == null || string.IsNullOrEmpty(leaveDTO.Reason))
            {
                return false;
            }
            return repository.AddLeave(leaveDTO);
        }

        [HttpPut("updateLeave")]
        public bool UpdateLeave([FromBody] LeaveDTO leaveDTO)
        {
            if (leaveDTO == null || leaveDTO.LeaveId <= 0)
            {
                return false;
            }
            return repository.UpdateLeave(leaveDTO);
        }

        [HttpDelete("deleteLeave/{id}")]
        public bool DeleteLeave(int id)
        {
            if (id <= 0)
            {
                return false;
            }
            return repository.DeleteLeave(id);
        }
    }
}
