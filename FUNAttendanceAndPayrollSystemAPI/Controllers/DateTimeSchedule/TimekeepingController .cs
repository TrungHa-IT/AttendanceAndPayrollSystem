using DataTransferObject.DateTimeDTO;
using Microsoft.AspNetCore.Mvc;
using Repository.DateTimeSchedule;

namespace FUNAttendanceAndPayrollSystemAPI.Controllers.DateTimeSchedule
{
    [Route("[controller]")]
    [ApiController]
    public class TimekeepingController : ControllerBase
    {
        private readonly ITimekeepingRepository _repo;

        public TimekeepingController(ITimekeepingRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("weeks")]
        public ActionResult<List<WeekOption>> GetWeeks([FromQuery] int year)
        {
            var weeks = _repo.GetWeeks(year);
            return Ok(weeks);
        }

        [HttpGet("days")]
        public ActionResult<List<DateTime>> GetDaysOfWeek([FromQuery] int year, [FromQuery] string week)
        {
            var days = _repo.GetDaysOfWeek(year, week);
            return Ok(days);
        }
    }
}
