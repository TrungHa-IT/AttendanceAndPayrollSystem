using BusinessObject.Models;
using DataAccess.ManagerDAO;
using DataTransferObject.EmployeeDTOS;
using DataTransferObject.ManagerDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ManagerRepository
{
    public class ManageEmployeeScheduleeRepository : IManageEmployeeScheduleRepository
    {
        public string CheckIn(int empId) => MangeScheduleEmployeeDAO.CheckIn(empId);

        public (bool success, string message) CheckInOT(int requestId)     => MangeScheduleEmployeeDAO.CheckInOT(requestId);


        public void CheckOut(int empId) => MangeScheduleEmployeeDAO.CheckOut(empId);

        public bool CheckOutOT(int requestId) => MangeScheduleEmployeeDAO.CheckOutOT(requestId);

        public List<DateOnly> CreateSchedulEmployee(int employeeId, DateOnly startDate, DateOnly endDate) => MangeScheduleEmployeeDAO.CreateScheduleForEmployee(employeeId, startDate, endDate);

        public void DeleteScheduleEmployee(int employeeId, DateOnly startDate, DateOnly endDate) => MangeScheduleEmployeeDAO.DeleteScheduleForEmployee(employeeId, startDate, endDate);

        public List<Schedule> GenerateFixedWeekSchedule(int employeeId, DateTime weekStart) => MangeScheduleEmployeeDAO.GenerateFixedWeekSchedule(employeeId, weekStart);

        public List<ApprovedOTDTO> GetApprovedOTDatesByEmployee(int employeeId) => MangeScheduleEmployeeDAO.GetApprovedOTDatesByEmployee(employeeId);

        public List<DateOnly> GetExistingScheduleDates(int employeeId, DateOnly startDate, DateOnly endDate) => MangeScheduleEmployeeDAO.GetExistingScheduleDates(employeeId, startDate, endDate);

        public List<ScheduleDTO> getListScheduleById(int employeeId) => MangeScheduleEmployeeDAO.getListScheduleById(employeeId);

        public bool HasCheckedInOTToday(int empId) => MangeScheduleEmployeeDAO.HasCheckedInOTToday(empId);

        public bool HasCheckedInToday(int empId) => MangeScheduleEmployeeDAO.HasCheckedInToday(empId);

        public List<BookingOTDTO> ManageOT() => MangeScheduleEmployeeDAO.ManageOT();

        public BookingOTDTO UpdateBooking(int otRequestId, string status, int approvedBy) => MangeScheduleEmployeeDAO.UpdateBooking(otRequestId, status, approvedBy);
    }
}
