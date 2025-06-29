using BusinessObject.Models;
using DataAccess.EmployeeDAO;
using DataTransferObject.AttendanceDTO;
using DataTransferObject.EmployeeDTO;
using DataTransferObject.EmployeeDTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.EmployeeRepository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        public BookingOTDTO bookScheduleOverTime(int emp, DateOnly otDate, TimeOnly startTime, TimeOnly endTime, string reason) => EmployeeDAO.bookScheduleOverTime(emp, otDate, startTime, endTime, reason);
        public List<AttendanceDTO> getAttendanceById(int emp) => EmployeeDAO.getAttendanceById(emp);
        public List<EmployeeDTO> GetEmployees() => EmployeeDAO.GetEmployees();

    }
}
