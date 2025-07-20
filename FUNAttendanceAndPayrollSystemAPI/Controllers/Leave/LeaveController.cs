using DataTransferObject.LeaveDTO;
using FUNAttendanceAndPayrollSystemAPI.Helpers;
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
        private readonly EmailService _emailService;

        public LeaveController(EmailService emailService)
        {
            _emailService = emailService;
        }

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
        public async Task<IActionResult> UpdateLeave([FromBody] LeaveDTO leaveDTO)
        {
            if (leaveDTO == null || leaveDTO.LeaveId <= 0)
                return BadRequest();
            var employeeEmail = repository.GetEmailEmployee(leaveDTO.LeaveId);
            leaveDTO.EmployeeEmail = employeeEmail;
            var result = repository.UpdateLeave(leaveDTO);
            if (!result)
                return BadRequest("Update failed.");

            string subject = $"Your Leave Request has been {leaveDTO.Status}";
            string body = $@"
            <h3>Hello {leaveDTO.EmployeeName},</h3>
            <p>Your leave request has been 
            <span style='color:{(leaveDTO.Status == "Approved" ? "green" : "red")}'><strong>{leaveDTO.Status}</strong></span>.</p>
            <p>Regards,<br/{leaveDTO.ApprovedByName} Staff Department></p>";

            await _emailService.SendEmailAsync(leaveDTO.EmployeeEmail, subject, body);

            return Ok("Leave updated and email sent.");
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
