using BusinessObject.Models;
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

        public static List<Employee> GetEmployees()
        {
            var listEmployees = new List<Employee>();
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    listEmployees = context.Employees.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listEmployees;
        }
    }
}
