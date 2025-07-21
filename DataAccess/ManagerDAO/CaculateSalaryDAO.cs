using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.ManagerDAO
{
    public class CaculateSalaryDAO
    {
        public static string CaculateSalary(int employeeId, int month, int year)
        {
            using var db = new FunattendanceAndPayrollSystemContext();

            var emp = db.Employees.FirstOrDefault(e => e.EmployId == employeeId);
            if (emp == null) return "Employee not found";

            var empAttendances = db.Attendances
                .Where(a => a.EmployeeId == employeeId && a.WorkDate.Month == month && a.WorkDate.Year == year)
                .ToList();

            double totalHours = empAttendances.Sum(a =>
            {
                if (a.CheckOut != null && a.CheckIn != null)
                    return (a.CheckOut.Value - a.CheckIn).TotalHours;
                return 0;
            });

            double totalSalary = totalHours * (double)emp.Salary;

            var existing = db.Payrolls.FirstOrDefault(p =>
                p.EmployeeId == employeeId && p.Month == month && p.Year == year);

            if (existing == null)
            {
                db.Payrolls.Add(new Payroll
                {
                    EmployeeId = employeeId,
                    Month = month,
                    Year = year,
                    TotalWorkHour = (decimal)totalHours,
                    TotalSalary = (decimal)totalSalary,
                });
            }
            else
            {
                existing.TotalWorkHour = (decimal)totalHours;
                existing.TotalSalary = (decimal)totalSalary;
            }

            db.SaveChanges();

            return $"Tính lương thành công cho {emp.EmployeeName} - Tháng {month} - {totalHours:0.00} giờ - {totalSalary:N0} VND";
        }
    }
}
