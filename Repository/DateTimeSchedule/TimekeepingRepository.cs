using DataAccess.DateTimeScheduleDAO;
using DataTransferObject.DateTimeDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DateTimeSchedule
{
    public class TimekeepingRepository : ITimekeepingRepository
    {
        private readonly TimeKeepDAO _dao = new ();

        public List<WeekOption> GetWeeks(int year)
        {
            return _dao.GetWeeksOfYear(year);
        }

        public List<DateTime> GetDaysOfWeek(int year, string weekDisplay)
        {
            var weeks = _dao.GetWeeksOfYear(year);
            var selectedWeek = weeks.FirstOrDefault(w => w.Display == weekDisplay);

            if (selectedWeek == null)
            {
                var today = DateTime.Today;
                selectedWeek = weeks.FirstOrDefault(w => today >= w.StartDate && today <= w.EndDate);
            }

            return selectedWeek != null ? _dao.GetDaysInWeek(selectedWeek.StartDate) : new List<DateTime>();
        }
    }
}
