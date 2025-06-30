using BusinessObject.Models;
using DataTransferObject.EmployeeDTOS;
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
        public IActionResult CheckInStatus([FromQuery] int empId)
        {
            var hasCheckedIn = repository.HasCheckedInToday(empId);
            var hasCheckedInOT = repository.HasCheckedInOTToday(empId); 

            return Ok(new
            {
                hasCheckedIn,
                hasCheckedInOT
            });
        }

        [HttpPost("checkIn")]
        public IActionResult CheckIn([FromBody] int empId)
        {

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
        public IActionResult CheckOut([FromBody] int empId)
        {
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

        [HttpGet("ManageOverTime")]
        public IActionResult ManageOverTime()
        {
            try
            {
                var listBooking = repository.ManageOT();
                if (listBooking == null) return NotFound();
                return Ok(listBooking);
            }catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPut("UpdateBooking")]
        public IActionResult UpdateBooking([FromBody] OTUpdateRequestDTO request)
        {
            try
            {
                var update = repository.UpdateBooking(request.Id, request.Status, request.EmpId);
                return Ok(update);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetScheduleApproved")]
        public IActionResult GetScheduleApproved([FromQuery] int employeeId)
        {
            try
            {
                var otRecords = repository.GetApprovedOTDatesByEmployee(employeeId);

                var result = otRecords.Select(o => new
                {
                    OvertimeRequestId = o.OvertimeRequestId,
                    OvertimeDate = o.OvertimeDate.ToString("yyyy-MM-dd")
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", detail = ex.Message });
            }
        }

        [HttpPost("CheckInOt")]
        public IActionResult CheckInOt([FromQuery] int requestId)
        {
            try
            {
                var checkInOt = repository.CheckInOT(requestId);

                return Ok(checkInOt);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", detail = ex.Message });
            }
        }

        [HttpPost("CheckOutOt")]
        public IActionResult CheckOutOt([FromQuery] int requestId)
        {
            try
            {
                var checkInOt = repository.CheckOutOT(requestId);

                return Ok(checkInOt);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error", detail = ex.Message });
            }
        }

    }
}
