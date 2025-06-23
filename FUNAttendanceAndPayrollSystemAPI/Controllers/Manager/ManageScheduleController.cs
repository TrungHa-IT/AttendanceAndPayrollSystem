using BusinessObject.Models;
using DataTransferObject.ManagerDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.ManagerRepository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FUNAttendanceAndPayrollSystemAPI.Controllers.Manager
{
    [Route("ManageSchedule/")]
    [ApiController]
    public class ManageScheduleController : ControllerBase
    {
        private readonly IManageEmployeeScheduleRepository repository = new ManageEmployeeScheduleeRepository();

        [HttpPost("CreateScheduleEmployee")]
        public IActionResult Post([FromBody] ScheduleRequestDTO request)
        {
            if (request.StartDate < DateOnly.FromDateTime(DateTime.Today))
                return BadRequest(new { message = "Start date cannot be in the past" });

            if (request.EndDate < request.StartDate)
                return BadRequest(new { message = "End date cannot be before start date" });

            var duplicatedDates = repository.CreateSchedulEmployee(request.EmployeeId, request.StartDate, request.EndDate);

            return Ok(new
            {
                message = "Schedule created",
                duplicated = duplicatedDates.Select(d => d.ToString("yyyy-MM-dd")).ToList()
            });
        }


        [HttpDelete("DeleteScheduleEmployee")]
        public IActionResult DeleteScheduleEmployee(
    [FromQuery] int employeeId,
    [FromQuery] DateOnly startDate,
    [FromQuery] DateOnly endDate)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var tomorrow = today.AddDays(1);

            if (startDate < tomorrow)
                return BadRequest(new { message = "You can only delete schedules starting from tomorrow." });

            if (endDate < startDate)
                return BadRequest(new { message = "End date cannot be before start date." });

            var existingDates = repository.GetExistingScheduleDates(employeeId, startDate, endDate);

            if (existingDates == null || existingDates.Count == 0)
            {
                return BadRequest(new { message = "No existing schedule found in the selected date range. Nothing to delete." });
            }

            repository.DeleteScheduleEmployee(employeeId, startDate, endDate);

            return Ok(new
            {
                message = "Schedule deleted successfully.",
                deletedDates = existingDates.Select(d => d.ToString("yyyy-MM-dd")).ToList()
            });
        }

        [HttpGet("GetScheduleByEmployee/{employeeId}")]

        public IActionResult GetListScheduleByEmployee(int employeeId)
        {
            try
            {
                if(employeeId == null)
                {
                    return BadRequest(new { message = "Employee id is null." });
                }
                var schedules = repository.getListScheduleById(employeeId);
                return Ok(schedules);
            }catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost("generate-week-schedule")]
        public IActionResult GenerateWeekSchedule([FromBody] DateTime weekStart)
        {
            var _dbContext = new FunattendanceAndPayrollSystemContext();
            var allEmployees = _dbContext.Employees
                                         .Where(ep => ep.Position.EndsWith("Employee"))
                                         .ToList();

            var skippedSchedules = new List<string>();

            foreach (var emp in allEmployees)
            {
                var schedules = repository.GenerateFixedWeekSchedule(emp.EmployId, weekStart);

                foreach (var sched in schedules)
                {
                    bool exists = _dbContext.Schedules.Any(s =>
                        s.EmployeeId == emp.EmployId &&
                        s.WorkDate == sched.WorkDate);

                    if (!exists)
                    {
                        _dbContext.Schedules.Add(sched);
                    }
                    else
                    {
                        skippedSchedules.Add($"Employee {emp.EmployId} already has schedule on {sched.WorkDate}");
                    }
                }
            }

            _dbContext.SaveChanges();

            if (skippedSchedules.Count > 0)
            {
                return Ok(new GenerateScheduleResponse
                {
                    Message = "Fixed weekly schedule generated (some skipped).",
                    Skipped = skippedSchedules
                });
            }

            return Ok(new GenerateScheduleResponse
            {
                Message = "Fixed weekly schedule generated.",
                Skipped = new List<string>()
            });
        }

        [HttpGet("checkInStatus")]
        public IActionResult CheckInStatus()
        {
            var empId = 6;
            bool hasCheckedIn = repository.HasCheckedInToday(empId);
            return Ok(new { hasCheckedIn });
        }


        [HttpPost("checkIn")]
        public IActionResult CheckIn()
        {
            int empId = 6;

            if (repository.HasCheckedInToday(empId))
            {
                return BadRequest(new { message = "Already checked in today." });
            }

            try
            {
                repository.CheckIn(empId);
                return Ok(new { message = "Check-in successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Check-in failed", error = ex.Message });
            }
        }

        [HttpPost("checkOut")]
        public IActionResult CheckOut()
        {
            int empId = 6;
            try
            {
                repository.CheckOut(empId);
                return Ok(new { message = "Check-in successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Check-in failed", error = ex.Message });
            }
        }



    }
}
