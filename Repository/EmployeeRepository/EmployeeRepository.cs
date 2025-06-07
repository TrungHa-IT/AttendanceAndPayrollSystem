using BusinessObject.Models;
using DataAccess.EmployeeDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.EmployeeRepository
{
    public class EmployeeRepository : IEmployeeRepository
    {

        public List<Employee> GetEmployees() => EmployeeDAO.GetEmployees();
    }
}
