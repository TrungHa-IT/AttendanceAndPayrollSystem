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
                    listEmployees = context.Employees.Include(emp => emp.Department)
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

        public static List<EmployeeDTO> GetStaffs()
        {
            var listEmployees = new List<EmployeeDTO>();
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    listEmployees = context.Employees.Include(emp => emp.Department).Where(ep => ep.Position.Equals("Staff"))
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

        public static List<EmployeeDTO> GetEmployeesTotalTimeByMonth(int? month, int? year, int? departmenId)
        {
            var listEmployees = new List<EmployeeDTO>();

            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    int currentMonth = month ?? DateTime.Now.Month;
                    int currentYear = year ?? DateTime.Now.Year;

                    var employees = context.Employees
                        .Include(emp => emp.Department)
                        .Where(ep => ep.Position.Equals("Employee") &&
                                    (departmenId == null || ep.DepartmentId == departmenId))
                        .ToList();

                    foreach (var emp in employees)
                    {
                        decimal totalPayrollHours = context.Payrolls
                            .Where(p => p.EmployeeId == emp.EmployId
                                     && p.Month == currentMonth
                                     && p.Year == currentYear)
                            .Sum(p => (decimal?)p.TotalWorkHour ?? 0);

                        var matchedOTs = context.OvertimeRequests
                            .Where(o => o.EmployeeId == emp.EmployId
                                     && o.OvertimeDate.Month == currentMonth
                                     && o.OvertimeDate.Year == currentYear
                                     && o.Status != null
                                     && (o.Status.ToLower() == "presented"))
                            .ToList();

                        decimal totalOTHours = matchedOTs.Sum(o => o.TotalHours ?? 0);
                        System.Diagnostics.Debug.WriteLine($"Employee {emp.EmployId} - Payroll: {totalPayrollHours}h, OT: {totalOTHours}h");

                        decimal totalTimeWorked = totalPayrollHours + totalOTHours;
                        decimal salaryPerHour = emp.Salary;
                        decimal totalSalary = (decimal)totalTimeWorked * salaryPerHour;

                        listEmployees.Add(new EmployeeDTO
                        {
                            EmployId = emp.EmployId,
                            EmployeeName = emp.EmployeeName,
                            Dob = emp.Dob,
                            Email = emp.Email,
                            PhoneNumber = emp.PhoneNumber,
                            Gender = emp.Gender,
                            Address = emp.Address,
                            Salary = emp.Salary,
                            Position = emp.Position,
                            DepartmentId = emp.DepartmentId,
                            DepartmentName = emp.Department?.DepartmentName,
                            PayrollTime = (double)totalPayrollHours,
                            OvertimeTime = (double)totalOTHours,
                            TotalTimeWorked = (double)totalTimeWorked,
                            TotalSalary = totalSalary
                        });
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error in GetEmployeesTotalTimeByMonth: " + e.Message);
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
                    Image = registerDTO.ImageUrl,
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
