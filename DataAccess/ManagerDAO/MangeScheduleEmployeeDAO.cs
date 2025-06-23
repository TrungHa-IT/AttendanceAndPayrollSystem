using BusinessObject.Models;
using DataTransferObject.ManagerDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ManagerDAO
{
    public class MangeScheduleEmployeeDAO
    {
        private readonly FunattendanceAndPayrollSystemContext _myDb;
        public MangeScheduleEmployeeDAO()
        {
            _myDb = new FunattendanceAndPayrollSystemContext();
        }

        public static List<ScheduleDTO> getListScheduleById(int employeeId)
        {
            try
            {
                using var db = new FunattendanceAndPayrollSystemContext();
                var listSchedules = db.Schedules.Where(sh => sh.EmployeeId == employeeId)
                    .Select(sh => new ScheduleDTO
                    {
                        ScheduleId = sh.ScheduleId,
                        EmployeeId = sh.EmployeeId,
                        WorkDate = sh.WorkDate.ToDateTime(TimeOnly.MinValue), 
                        StartTime = sh.StartTime,
                        EndTime = sh.EndTime
                    }).ToList();
                return listSchedules;
            }catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static List<Schedule> GenerateFixedWeekSchedule(int employeeId, DateTime weekStart)
        {
            var schedules = new List<Schedule>();
            var startTime = new TimeSpan(8, 0, 0); 
            var endTime = new TimeSpan(17, 0, 0);   

            for (int i = 0; i < 5; i++) 
            {
                var workDate = weekStart.AddDays(i); 

                schedules.Add(new Schedule
                {
                    EmployeeId = employeeId,
                    WorkDate = DateOnly.FromDateTime(workDate),
                    StartTime = TimeOnly.FromTimeSpan(startTime),
                    EndTime = TimeOnly.FromTimeSpan(endTime)
                });
            }

            return schedules;
        }
        public static List<DateOnly> CreateScheduleForEmployee(int employeeId, DateOnly startDate, DateOnly endDate)
        {
            using var context = new FunattendanceAndPayrollSystemContext();

            var duplicatedDates = context.Schedules
                .Where(s => s.EmployeeId == employeeId && s.WorkDate >= startDate && s.WorkDate <= endDate)
                .Select(s => s.WorkDate)
                .ToList();

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (duplicatedDates.Contains(date)) continue;

                var schedule = new Schedule
                {
                    EmployeeId = employeeId,
                    WorkDate = date,
                    StartTime = new TimeOnly(8, 0),
                    EndTime = new TimeOnly(17, 0)
                };

                context.Schedules.Add(schedule);
            }

            context.SaveChanges();
            return duplicatedDates; 
        }

        public static void DeleteScheduleForEmployee(int employeeId, DateOnly startDate, DateOnly endDate)
        {
            using var context = new FunattendanceAndPayrollSystemContext();

            var schedulesToDelete = context.Schedules
                .Where(s => s.EmployeeId == employeeId && s.WorkDate >= startDate && s.WorkDate <= endDate)
                .ToList();

            if (schedulesToDelete.Any())
            {
                context.Schedules.RemoveRange(schedulesToDelete);
                context.SaveChanges();
            }
        }

        public static List<DateOnly> GetExistingScheduleDates(int employeeId, DateOnly startDate, DateOnly endDate)
        {
            using var context = new FunattendanceAndPayrollSystemContext();

            var existingDates = context.Schedules
                .Where(s => s.EmployeeId == employeeId && s.WorkDate >= startDate && s.WorkDate <= endDate)
                .Select(s => s.WorkDate)
                .ToList();

            return existingDates;
        }

        public static bool HasCheckedInToday(int empId)
        {
            using var contextDB = new FunattendanceAndPayrollSystemContext();
            var today = DateOnly.FromDateTime(DateTime.Today);

            return contextDB.Attendances.Any(a => a.EmployeeId == empId && a.WorkDate == today);
        }


        public static void CheckIn(int empId)
        {
            using var contextDB = new FunattendanceAndPayrollSystemContext();
            var now = DateTime.Now;
            var attendance = new Attendance
            {
                EmployeeId = empId,
                WorkDate = DateOnly.FromDateTime(now.Date),
                CheckIn = TimeOnly.FromDateTime(now),

            };
            contextDB.Attendances.Add(attendance);
            contextDB.SaveChanges();
        }


        public static void CheckOut(int empId)
        {
            using var contextDB = new FunattendanceAndPayrollSystemContext();

            var today = DateOnly.FromDateTime(DateTime.Now.Date);

            var attendance = contextDB.Attendances
                .FirstOrDefault(a => a.EmployeeId == empId && a.WorkDate == today);

            if (attendance == null)
            {
                throw new InvalidOperationException("You must check in before checking out.");
            }

            attendance.CheckOut = TimeOnly.FromDateTime(DateTime.Now);

            contextDB.SaveChanges();
        }

    }
}
