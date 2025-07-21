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

            double totalMinutes = empAttendances.Sum(a =>
            {
                if (a.CheckIn != null && a.CheckOut != null)
                    return (a.CheckOut.Value - a.CheckIn).TotalMinutes;
                return 0;
            });

            double salaryPerMinute = (double)emp.Salary / (24 * 8 * 60);
            double totalSalary = totalMinutes * salaryPerMinute;

            var existing = db.Payrolls.FirstOrDefault(p =>
                p.EmployeeId == employeeId && p.Month == month && p.Year == year);

            if (existing == null)
            {
                db.Payrolls.Add(new Payroll
                {
                    EmployeeId = employeeId,
                    Month = month,
                    Year = year,
                    TotalWorkHour = (decimal)(totalMinutes / 60),
                    TotalSalary = (decimal)totalSalary,
                });
            }
            else
            {
                existing.TotalWorkHour = (decimal)(totalMinutes / 60);
                existing.TotalSalary = (decimal)totalSalary;
            }

            db.SaveChanges();

            return $"Tính lương thành công cho {emp.EmployeeName} - Tháng {month} - {(totalMinutes / 60):0.00} giờ - {totalSalary:N0} VND";
        }

    }
}
