using BusinessObject.Models;
using DataAccess.EmployeeDAO;
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
    public class EmployeeRepository : IEmployeeRepository
    {
        public List<AttendanceDTO> getAttendanceById(int emp) => EmployeeDAO.getAttendanceById(emp);
        public List<EmployeeDTO> GetEmployees() => EmployeeDAO.GetEmployees();

        public Employee? Login(LoginDTO loginDTO) => EmployeeDAO.Login(loginDTO);
        
        public bool Register(RegisterDTO registerDTO) => EmployeeDAO.Register(registerDTO);
    }
}
