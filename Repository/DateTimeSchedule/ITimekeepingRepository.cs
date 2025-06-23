using DataTransferObject.DateTimeDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DateTimeSchedule
{
    public interface ITimekeepingRepository
    {
        List<WeekOption> GetWeeks(int year);
        List<DateTime> GetDaysOfWeek(int year, string weekDisplay);
    }
}
