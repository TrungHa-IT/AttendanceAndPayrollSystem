using BusinessObject.Models;
using DataTransferObject.DepartmentDTO;
using DataTransferObject.EmployeeDTOS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DepartmentDAO
{
    public class DepartmentDAO
    {
        private readonly FunattendanceAndPayrollSystemContext _MyDb;
        public DepartmentDAO()
        {
            _MyDb = new FunattendanceAndPayrollSystemContext();
        }

        //get all departments
        public static List<DepartmentDTO> GetDepartments()
        {
            var listDepartments = new List<DepartmentDTO>();
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    listDepartments = context.Departments.Select(at => new DepartmentDTO
                    {
                        DepartmentId = at.DepartmentId,
                        DepartmentName = at.DepartmentName,
                        Description = at.Description,
                        CreatedAt = at.CreatedAt,
                        UpdatedAt = at.UpdatedAt,
                        DeletedAt = at.DeletedAt
                    }).ToList(); ;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listDepartments;
        }

        // Create a new department
        public static bool AddDepartment(DepartmentDTO departmentDTO)
        {
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    var department = new Department
                    {
                        DepartmentName = departmentDTO.DepartmentName,
                        Description = departmentDTO.Description,
                        CreatedAt = DateTime.Now
                    };

                    context.Departments.Add(department);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                // Log the exception if needed
                return false;
            }
        }

        // Update an existing department
        public static bool UpdateDepartment(DepartmentDTO departmentDTO)
        {
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    var department = context.Departments.Find(departmentDTO.DepartmentId);
                    if (department == null || department.DeletedAt != null)
                    {
                        return false; // Not found or already deleted
                    }

                    department.DepartmentName = departmentDTO.DepartmentName;
                    department.Description = departmentDTO.Description;
                    department.UpdatedAt = DateTime.Now;

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                // Log the exception if needed
                return false;
            }
        }

        // Soft delete a department
        public static bool DeleteDepartment(int departmentId)
        {
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    var department = context.Departments.FirstOrDefault(d => d.DepartmentId == departmentId && d.DeletedAt == null);

                    if (department == null)
                    {
                        return false; 
                    }
                    department.DeletedAt = DateTime.Now;
                    context.Remove(department);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                // Log the exception if needed
                return false;
            }
        }

    }
}
