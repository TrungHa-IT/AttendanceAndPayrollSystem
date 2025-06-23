using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.DateTimeDTO;

namespace DataAccess.DateTimeScheduleDAO
{
    public class TimeKeepDAO
    {
        public  List<WeekOption> GetWeeksOfYear(int year)
        {
            List<WeekOption> result = new();
            DateTime firstMonday = new DateTime(year, 1, 1);

            while (firstMonday.DayOfWeek != DayOfWeek.Monday)
                firstMonday = firstMonday.AddDays(1);

            while (firstMonday.Year <= year)
            {
                var end = firstMonday.AddDays(6);
                if (end.Year > year) break;

                result.Add(new WeekOption
                {
                    StartDate = firstMonday,
                    EndDate = end
                });

                firstMonday = firstMonday.AddDays(7);
            }

            return result;
        }

        public  List<DateTime> GetDaysInWeek(DateTime startDate)
        {
            return Enumerable.Range(0, 7)
                             .Select(i => startDate.AddDays(i))
                             .ToList();
        }
    }
}

