using BusinessObject.Models;
using DataTransferObject.DepartmentDTO;
using DataTransferObject.LeaveTypeDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.LeaveTypeDAO
{
    public class LeaveTypeDAO
    {
        private readonly FunattendanceAndPayrollSystemContext _MyDb;
        public LeaveTypeDAO()
        {
            _MyDb = new FunattendanceAndPayrollSystemContext();
        }

        //get all leave types
        public static List<LeaveTypeDTO> GetLeaveType()
        {
            var listLeaveType = new List<LeaveTypeDTO>();
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    listLeaveType = context.LeaveTypes.Select(at => new LeaveTypeDTO
                    {
                        LeaveTypeId = at.LeaveTypeId,
                        LeaveTypeName = at.LeaveTypeName,
                        IsPaid = at.IsPaid,
                        MaxDaysAllowed = at.MaxDaysAllowed,
                        CreatedAt = at.CreatedAt,
                        UpdatedAt = at.UpdatedAt,
                        DeletedAt = at.DeletedAt
                    }).ToList(); 
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listLeaveType;
        }

        // Create a new leave type
        public static bool AddLeaveType(LeaveTypeDTO leaveTypeDTO)
        {
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    var leaveType = new LeaveType
                    {
                        LeaveTypeName = leaveTypeDTO.LeaveTypeName,
                        IsPaid = leaveTypeDTO.IsPaid,
                        MaxDaysAllowed = leaveTypeDTO.MaxDaysAllowed,
                        CreatedAt = DateTime.Now,   
                    };

                    context.LeaveTypes.Add(leaveType);
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

        // Update an existing leave type
        public static bool UpdateLeaveType(LeaveTypeDTO leaveTypeDTO)
        {
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    var leaveType = context.LeaveTypes.Find(leaveTypeDTO.LeaveTypeId);
                    if (leaveType == null || leaveType.DeletedAt != null)
                    {
                        return false; // Not found or already deleted
                    }

                    leaveType.LeaveTypeName = leaveTypeDTO.LeaveTypeName;
                    leaveType.IsPaid = leaveTypeDTO.IsPaid;
                    leaveType.MaxDaysAllowed = leaveTypeDTO.MaxDaysAllowed;
                    leaveType.UpdatedAt = DateTime.Now;

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

        // Hard delete a leave type
        public static bool DeleteLeaveType(int leaveTypeID)
        {
            try
            {
                using var context = new FunattendanceAndPayrollSystemContext();

                var leaveType = context.LeaveTypes.FirstOrDefault(l => l.LeaveTypeId == leaveTypeID);

                if (leaveType == null)
                {
                    return false; 
                }

                context.LeaveTypes.Remove(leaveType); 
                context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
