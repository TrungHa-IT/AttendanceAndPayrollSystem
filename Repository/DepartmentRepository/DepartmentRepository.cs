using DataAccess.DepartmentDAO;
using DataTransferObject.DepartmentDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DepartmentRepository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        public bool AddDepartment(DepartmentDTO department) => DepartmentDAO.AddDepartment(department);

        public bool DeleteDepartment(int id) => DepartmentDAO.DeleteDepartment(id);

        public List<DepartmentDTO> GetDepartments() => DepartmentDAO.GetDepartments();

        public bool UpdateDepartment(DepartmentDTO department) => DepartmentDAO.UpdateDepartment(department);
    }
}
