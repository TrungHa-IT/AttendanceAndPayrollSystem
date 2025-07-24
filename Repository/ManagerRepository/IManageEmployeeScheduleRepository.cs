using BusinessObject.Models;
using DataTransferObject.AttendanceDTO;
using DataTransferObject.EmployeeDTOS;
using DataTransferObject.ManagerDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ManagerRepository
{
    public interface IManageEmployeeScheduleRepository
    {
        List<DateOnly> CreateSchedulEmployee(int employeeId, DateOnly startDate, DateOnly endDate);
        void DeleteScheduleEmployee(int employeeId, DateOnly startDate, DateOnly endDate);
        List<DateOnly> GetExistingScheduleDates(int employeeId, DateOnly startDate, DateOnly endDate);
        List<ScheduleDTO> getListScheduleById(int employeeId);
        List<Schedule> GenerateFixedWeekSchedule(int employeeId, DateTime weekStart);
        string CheckIn(int empId);
        bool HasCheckedInToday (int empId);
        bool HasCheckedInOTToday(int empId);
        void CheckOut(int empId);
        List<BookingOTDTO> ManageOT();
        BookingOTDTO UpdateBooking(int otRequestId, string status, int approvedBy);
        List<ApprovedOTDTO> GetApprovedOTDatesByEmployee(int employeeId);
        (bool success, string message) CheckInOT(int requestId);

        bool CheckOutOT(int requestId);
        List<OTStatusDTO> GetStatusOT(int empId);
    }
}
