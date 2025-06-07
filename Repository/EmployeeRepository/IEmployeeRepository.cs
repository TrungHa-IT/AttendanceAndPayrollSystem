using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.EmployeeRepository
{
    public interface IEmployeeRepository
    {
        List<Employee> GetEmployees();
    }
}
