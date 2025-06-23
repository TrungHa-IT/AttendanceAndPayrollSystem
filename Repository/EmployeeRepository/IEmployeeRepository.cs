using BusinessObject.Models;
using DataTransferObject.AttendanceDTO;
using DataTransferObject.AuthDTO;
using DataTransferObject.EmployeeDTO;
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
        bool Register(RegisterDTO registerDTO);
        Employee? Login(LoginDTO loginDTO);
    }


}
