using BusinessObject.Models;
using DataAccess.ManagerDAO;
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
        public void CheckIn(int empId) => MangeScheduleEmployeeDAO.CheckIn(empId);

        public void CheckOut(int empId) => MangeScheduleEmployeeDAO.CheckOut(empId);
        public List<DateOnly> CreateSchedulEmployee(int employeeId, DateOnly startDate, DateOnly endDate) => MangeScheduleEmployeeDAO.CreateScheduleForEmployee(employeeId, startDate, endDate);

        public void DeleteScheduleEmployee(int employeeId, DateOnly startDate, DateOnly endDate) => MangeScheduleEmployeeDAO.DeleteScheduleForEmployee(employeeId, startDate, endDate);

        public List<Schedule> GenerateFixedWeekSchedule(int employeeId, DateTime weekStart) => MangeScheduleEmployeeDAO.GenerateFixedWeekSchedule(employeeId, weekStart);

        public List<DateOnly> GetExistingScheduleDates(int employeeId, DateOnly startDate, DateOnly endDate) => MangeScheduleEmployeeDAO.GetExistingScheduleDates(employeeId, startDate, endDate);

        public List<ScheduleDTO> getListScheduleById(int employeeId) => MangeScheduleEmployeeDAO.getListScheduleById(employeeId);

        public bool HasCheckedInToday(int empId) => MangeScheduleEmployeeDAO.HasCheckedInToday(empId);
    }
}
