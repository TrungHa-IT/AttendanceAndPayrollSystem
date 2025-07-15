using BusinessObject.Models;
using DataTransferObject.EmployeeDTOS;
using DataTransferObject.ManagerDTO;
using Microsoft.EntityFrameworkCore;
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

        public static bool HasCheckedInOTToday(int empId)
        {
            using var context = new FunattendanceAndPayrollSystemContext();
            var today = DateOnly.FromDateTime(DateTime.Today);

            return context.OvertimeRequests.Any(o =>
                o.EmployeeId == empId &&
                o.OvertimeDate == today &&
                o.IsCheckIn == true);
        }



        public static string CheckIn(int empId)
        {
            using var contextDB = new FunattendanceAndPayrollSystemContext();
            var now = DateTime.Now;

            var checkInTime = TimeOnly.FromDateTime(now);
            var earliest = new TimeOnly(8, 30);
            var latestAllowed = new TimeOnly(14, 0);
            var autoSetTime = new TimeOnly(12, 0);
            var lateThreshold = new TimeOnly(10, 30);

            if (checkInTime > latestAllowed)
            {
                return "Check-in not allowed after 2:00 PM.";
            }

            if (checkInTime > lateThreshold)
            {
                checkInTime = autoSetTime;
            }
            else if (checkInTime < earliest)
            {
                checkInTime = earliest;
            }

            var attendance = new Attendance
            {
                EmployeeId = empId,
                WorkDate = DateOnly.FromDateTime(now.Date),
                CheckIn = checkInTime,
            };

            contextDB.Attendances.Add(attendance);
            contextDB.SaveChanges();

            return "Check-in successful";
        }




        public static void CheckOut(int empId)
        {
            using var contextDB = new FunattendanceAndPayrollSystemContext();

            var today = DateOnly.FromDateTime(DateTime.Now.Date);
            var now = DateTime.Now;

            var attendance = contextDB.Attendances
                .FirstOrDefault(a => a.EmployeeId == empId && a.WorkDate == today);

            if (attendance == null)
            {
                throw new InvalidOperationException("You must check in before checking out.");
            }

            var checkOutTime = TimeOnly.FromDateTime(now);
            var latest = new TimeOnly(17, 30); 

            if (checkOutTime > latest)
            {
                checkOutTime = latest;
            }

            attendance.CheckOut = checkOutTime;
            contextDB.SaveChanges();
        }


        public static List<BookingOTDTO> ManageOT()
        {
            using var _db = new FunattendanceAndPayrollSystemContext();
            try
            {
                var listBooking = _db.OvertimeRequests.Include(ot => ot.Employee).Where(ot => ot.Status.Equals("processing"))
                    .Select(ot => new BookingOTDTO
                    {
                        OvertimeRequestId = ot.OvertimeRequestId,
                        EmployeeId = ot.EmployeeId,
                        StartTime = ot.StartTime,
                        EndTime = ot.EndTime,
                        Status = ot.Status,
                        Reason = ot.Reason,
                        OvertimeDate = ot.OvertimeDate
                    }).ToList();
                return listBooking;
            }catch(Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public static BookingOTDTO UpdateBooking(int otRequestId, string status, int approvedBy)
        {
            using var _db = new FunattendanceAndPayrollSystemContext();

            try
            {
                var ot = _db.OvertimeRequests.FirstOrDefault(o => o.OvertimeRequestId == otRequestId);

                if (ot == null)
                    throw new Exception("Overtime request not found");

                ot.Status = status;
                ot.ApprovedBy = approvedBy;
                ot.ApprovedDate = DateTime.Now;
                ot.UpdatedAt = DateTime.Now;

                _db.SaveChanges();

                return new BookingOTDTO
                {
                    OvertimeRequestId = ot.OvertimeRequestId,
                    EmployeeId = ot.EmployeeId,
                    OvertimeDate = ot.OvertimeDate,
                    StartTime = ot.StartTime,
                    EndTime = ot.EndTime,
                    Reason = ot.Reason,
                    Status = ot.Status
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating OT request: " + ex.Message);
            }
        }

        public static List<ApprovedOTDTO> GetApprovedOTDatesByEmployee(int employeeId)
        {
            using var _context = new FunattendanceAndPayrollSystemContext();
            return _context.OvertimeRequests
                .Where(o => o.EmployeeId == employeeId && o.Status.ToLower() == "approved")
                .Select(o => new ApprovedOTDTO
                {
                    OvertimeRequestId = o.OvertimeRequestId,
                    OvertimeDate = o.OvertimeDate
                })
                .ToList();
        }

        public static bool CheckInOT(int requestId)
        {
            using var _context = new FunattendanceAndPayrollSystemContext();
            var otRequest =  _context.OvertimeRequests.FirstOrDefault(ot => ot.OvertimeRequestId == requestId);

            if (otRequest == null || otRequest.Status != "approved")
                return false;

            DateTime now = DateTime.Now;

            DateTime registeredStart = otRequest.OvertimeDate.ToDateTime(otRequest.StartTime);

            TimeOnly finalStartTime = (now < registeredStart) ? otRequest.StartTime : TimeOnly.FromDateTime(now);

            otRequest.StartTime = finalStartTime;
            otRequest.IsCheckIn = true;
            otRequest.UpdatedAt = DateTime.Now;

             _context.SaveChangesAsync();
            return true;
        }

        public static bool CheckOutOT(int requestId)
        {
            using var _context = new FunattendanceAndPayrollSystemContext();
            var otRequest = _context.OvertimeRequests.FirstOrDefault(ot => ot.OvertimeRequestId == requestId);

            if (otRequest == null || otRequest.Status != "approved")
                return false;

            DateTime now = DateTime.Now;

            DateTime registeredStart = otRequest.OvertimeDate.ToDateTime(otRequest.StartTime);
            DateTime registeredEnd = otRequest.OvertimeDate.ToDateTime(otRequest.EndTime);

            DateTime actualEnd = now < registeredEnd ? now : registeredEnd;

            if (actualEnd <= registeredStart)
                return false;

            TimeOnly finalEndTime = TimeOnly.FromDateTime(actualEnd);
            TimeSpan duration = actualEnd - registeredStart;

            otRequest.EndTime = finalEndTime;
            otRequest.TotalHours = (decimal)duration.TotalHours;
            otRequest.Status = "presented";
            otRequest.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            return true;
        }


    }
}
