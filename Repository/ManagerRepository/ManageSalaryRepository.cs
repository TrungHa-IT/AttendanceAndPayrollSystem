using DataAccess.ManagerDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ManagerRepository
{
    public class ManageSalaryRepository : IManageSalaryRepository
    {
        public string CaculateSalary(int employeeId, int month, int year) => CaculateSalaryDAO.CaculateSalary(employeeId, month, year);
    }
}
