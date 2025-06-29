using BusinessObject.Models;
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
    public interface IEmployeeRepository
    {
        List<EmployeeDTO> GetEmployees();
        List<AttendanceDTO> getAttendanceById(int emp);
        BookingOTDTO bookScheduleOverTime(int emp, DateOnly otDate, TimeOnly startTime, TimeOnly endTime, string reason);
    }


}
