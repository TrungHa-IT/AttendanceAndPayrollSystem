using BusinessObject.Models;
using DataTransferObject.AttendanceDTO;
using DataTransferObject.AuthDTO;
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
        List<EmployeeDTO> GetStaffs();
        List<AttendanceDTO> getAttendanceById(int emp);
        BookingOTDTO bookScheduleOverTime(int emp, DateOnly otDate, TimeOnly startTime, TimeOnly endTime, string reason);
        bool Register(RegisterDTO registerDTO);
        Employee? Login(LoginDTO loginDTO);
        List<EmployeeDTO> GetEmployeesTotalTimeByMonth(int? month, int? year, int? departmenId);
    }


}
