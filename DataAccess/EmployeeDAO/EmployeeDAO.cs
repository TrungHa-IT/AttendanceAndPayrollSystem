using BusinessObject.Models;
using DataTransferObject.AttendanceDTO;
using DataTransferObject.EmployeeDTO;
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
    }
}
