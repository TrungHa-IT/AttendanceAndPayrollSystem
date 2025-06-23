using DataTransferObject.DepartmentDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DepartmentRepository
{
    public interface IDepartmentRepository
    {
        List<DepartmentDTO> GetDepartments();
        bool AddDepartment(DepartmentDTO department);
        bool UpdateDepartment(DepartmentDTO department);
        bool DeleteDepartment(int id);
    }
}
