using BusinessObject.Models;
using DataAccess.EmployeeDAO;
using DataTransferObject.AttendanceDTO;
using DataTransferObject.AuthDTO;
using DataTransferObject.EmployeeDTOS;
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

        public bool DeleteEmployee(int id) => EmployeeDAO.DeleteEmployee(id);

        public List<AttendanceDTO> getAttendanceById(int emp, int? month, int? year) => EmployeeDAO.getAttendanceById(emp, month,year);
        public List<EmployeeDTO> GetEmployees() => EmployeeDAO.GetEmployees();

        public List<EmployeeDTO> GetEmployeesTotalTimeByMonth(int? month, int? year, int? departmenId) => EmployeeDAO.GetEmployeesTotalTimeByMonth(month, year, departmenId);

        public EmployeeProfileDTO getInformationProfile(int empId) => EmployeeDAO.getInformationProfile(empId);

        public List<EmployeeDTO> GetStaffs() => EmployeeDAO.GetStaffs();

        public Employee? Login(LoginDTO loginDTO) => EmployeeDAO.Login(loginDTO);
        
        public bool Register(RegisterDTO registerDTO) => EmployeeDAO.Register(registerDTO);

        public bool UpdateBasicInfo(Employee updated) => EmployeeDAO.UpdateBasicInfo(updated);

        public bool UpdateCertificates(int employeeId, List<EmployeeCertificate> newCertificates) => EmployeeDAO.UpdateCertificates(employeeId, newCertificates);

        public bool UpdateEmployee(EmployeeUpdateDTO updatedEmployee) => EmployeeDAO.UpdateEmployee(updatedEmployee);

        public bool UpdateSkills(int employeeId, List<EmployeeSkill> newSkills) => EmployeeDAO.UpdateSkills(employeeId, newSkills);
    }
}
