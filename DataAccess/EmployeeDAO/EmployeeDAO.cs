using BusinessObject.Models;
using DataTransferObject.AttendanceDTO;
using DataTransferObject.AuthDTO;
using DataTransferObject.EmployeeDTO;
using DataTransferObject.EmployeeDTOS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EmployeeDAO
{
    public class EmployeeDAO
    {
        private readonly FunattendanceAndPayrollSystemContext _MyDb;
        public EmployeeDAO()
        {
            _MyDb = new FunattendanceAndPayrollSystemContext();
        }

        public static List<EmployeeDTO> GetEmployees()
        {
            var listEmployees = new List<EmployeeDTO>();
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    listEmployees = context.Employees.Include(emp => emp.Department).Where(ep => ep.Position.Equals("Employee"))
                        .Select(ep => new EmployeeDTO
                        {
                            EmployId = ep.EmployId,
                            EmployeeName = ep.EmployeeName,
                            Dob = ep.Dob,
                            Email = ep.Email,
                            PhoneNumber = ep.PhoneNumber,
                            Gender = ep.Gender,
                            Address = ep.Address,
                            Position = ep.Position,
                            DepartmentId = ep.DepartmentId,
                            DepartmentName = ep.Department.DepartmentName
                        }).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listEmployees;
        }

        public static List<EmployeeDTO> GetEmployeesTotalTimeByMonth(int? month, int? year)
        {
            var listEmployees = new List<EmployeeDTO>();

            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    int currentMonth = month ?? DateTime.Now.Month;
                    int currentYear = year ?? DateTime.Now.Year;

                    listEmployees = context.Employees
                        .Include(emp => emp.Department)
                        .Where(ep => ep.Position.Equals("Employee"))
                        .Select(ep => new EmployeeDTO
                        {
                            EmployId = ep.EmployId,
                            EmployeeName = ep.EmployeeName,
                            Dob = ep.Dob,
                            Email = ep.Email,
                            PhoneNumber = ep.PhoneNumber,
                            Gender = ep.Gender,
                            Address = ep.Address,
                            Salary = ep.Salary,
                            Position = ep.Position,
                            DepartmentId = ep.DepartmentId,
                            DepartmentName = ep.Department.DepartmentName,

                            TotalTimeWorked =
                                context.Payrolls
                                    .Where(p => p.EmployeeId == ep.EmployId
                                             && p.Month == currentMonth
                                             && p.Year == currentYear)
                                    .Sum(p => (double?)p.TotalWorkHour ?? 0)
                                +
                                context.OvertimeRequests
                                    .Where(o => o.EmployeeId == ep.EmployId
                                             && o.OvertimeDate.Month == currentMonth
                                             && o.OvertimeDate.Year == currentYear
                                             && o.Status == "success")
                                    .Sum(o => (double?)o.TotalHours ?? 0)
                        })
                        .ToList();
                    foreach (var emp in listEmployees)
                    {
                        emp.TotalSalary = (decimal)emp.TotalTimeWorked * emp.Salary.GetValueOrDefault(0);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return listEmployees;
        }

        public static List<AttendanceDTO> getAttendanceById(int emp)
        {
            using var _db = new FunattendanceAndPayrollSystemContext();
            try
            {
                var attendanceEmp = _db.Attendances.Include(at => at.Employee)
                    .ThenInclude(emp => emp.Department)
                    .Where(at => at.EmployeeId == emp)
                    .Select(at => new AttendanceDTO
                    {
                        EmployeeId = at.EmployeeId,
                        EmployeeName = at.Employee.EmployeeName,
                        DepartmentName = at.Employee.Department.DepartmentName,
                        WorkDate = at.WorkDate,
                        CheckIn = at.CheckIn,
                        CheckOut = at.CheckOut
                    }).ToList();
                return attendanceEmp;

            }catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // Register
        public static bool Register(RegisterDTO registerDTO)
        {
            using var _MyDb = new FunattendanceAndPayrollSystemContext();
            try
            {
                // Check if email already exists
                var existingUser = _MyDb.Employees.FirstOrDefault(e => e.Email == registerDTO.Email);
                if (existingUser != null) return false;

                // Create new Employee
                var newEmployee = new Employee
                {
                    EmployeeName = registerDTO.EmployeeName,
                    Image = registerDTO.Image,
                    Dob = registerDTO.Dob,
                    Email = registerDTO.Email,
                    PhoneNumber = registerDTO.PhoneNumber,
                    Gender = registerDTO.Gender,
                    Address = registerDTO.Address,
                    Position = registerDTO.Position ?? "Employee",
                    Salary = registerDTO.Salary ?? 0,
                    Status = registerDTO.Status,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = null,
                    DeletedAt = null,
                    DepartmentId = registerDTO.DepartmentId,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password) 
                };

                _MyDb.Employees.Add(newEmployee);
                _MyDb.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Login
        public static Employee? Login(LoginDTO loginDTO)
        {
            using var _MyDb = new FunattendanceAndPayrollSystemContext();
            try
            {
                var employee = _MyDb.Employees.FirstOrDefault(e => e.Email == loginDTO.Email);
                if (employee == null) return null;

                //check password
                bool isValidPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, employee.Password);

                return isValidPassword ? employee : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static BookingOTDTO bookScheduleOverTime(int emp,DateOnly otDate, TimeOnly startTime, TimeOnly endTime,string reason)
        {
            using var _db = new FunattendanceAndPayrollSystemContext();
            try
            {
                var newRequest = new OvertimeRequest
                {
                    EmployeeId = emp,
                    OvertimeDate = otDate,
                    StartTime = startTime,
                    EndTime = endTime,
                    Reason = reason,
                    Status = "processing"
                };
                _db.OvertimeRequests.Add(newRequest);
                _db.SaveChanges();
                return new BookingOTDTO
                {
                    EmployeeId = newRequest.EmployeeId,
                    OvertimeDate = newRequest.OvertimeDate,
                    StartTime = newRequest.StartTime,
                    EndTime = newRequest.EndTime,
                    Status = newRequest.Status,
                    Reason = newRequest.Reason
                };
            }catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
