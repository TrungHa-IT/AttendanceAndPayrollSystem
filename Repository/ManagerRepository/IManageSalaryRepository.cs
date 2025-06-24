using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ManagerRepository
{
    public interface IManageSalaryRepository
    {
        string CaculateSalary(int employeeId, int month, int year);
    }
}
