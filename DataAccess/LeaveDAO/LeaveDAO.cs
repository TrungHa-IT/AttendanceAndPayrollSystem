using BusinessObject.Models;
using DataTransferObject.LeaveDTO;
using DataTransferObject.LeaveTypeDTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.LeaveDAO
{
    public class LeaveDAO
    {
        private readonly FunattendanceAndPayrollSystemContext _MyDb;
        public LeaveDAO()
        {
            _MyDb = new FunattendanceAndPayrollSystemContext();
        }

        //get all leave 
        public static List<LeaveDTO> GetLeaves()
        {
            var listLeaves = new List<LeaveDTO>();
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    listLeaves = context.Leaves.Select(at => new LeaveDTO
                    {
                       ApprovedBy = at.ApprovedBy,
                       DurationInDays = at.DurationInDays,
                       EmployeeId = at.EmployeeId,
                       EndDate = at.EndDate,
                       LeaveId = at.LeaveId,
                       LeaveTypeId = at.LeaveTypeId,
                       Reason = at.Reason,
                       StartDate = at.StartDate,
                       Status = at.Status,
                       ApprovedByName = at.Employee.EmployeeName,
                       ApprovedDate = at.ApprovedDate,
                    }).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listLeaves;
        }

        // Get all leave records of a specific employee, ordered by StartDate descending
        public static List<LeaveDTO> GetLeaveEmployee(int id)
        {
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    var listLeaves = context.Leaves
                        .Where(e => e.EmployeeId == id)
                        .OrderByDescending(e => e.StartDate)
                        .Select(at => new LeaveDTO
                        {
                            LeaveId = at.LeaveId,
                            EmployeeId = at.EmployeeId,
                            LeaveTypeId = at.LeaveTypeId,
                            StartDate = at.StartDate,
                            EndDate = at.EndDate,
                            DurationInDays = at.DurationInDays,
                            Reason = at.Reason,
                            Status = at.Status,
                            ApprovedBy = at.ApprovedBy,
                            LeaveTypeName = at.LeaveType.LeaveTypeName,
                            ApprovedByName = at.Employee.EmployeeName 
                        }).ToList();

                    return listLeaves;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error retrieving leave records: " + e.Message);
            }
        }

        //Get email of employee when approve leave
        public static string GetLeaveEmployeeEmail(int leaveID)
        {
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    var email = context.Leaves
                        .Include(l => l.Employee)
                        .Where(l => l.LeaveId == leaveID)
                        .Select(l => l.Employee.Email)
                        .FirstOrDefault(); 

                    return email ?? string.Empty;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error retrieving leave records: " + e.Message);
            }
        }

        // Get all leave approve by staff
        public static List<LeaveDTO> GetLeaveStaff(int id)
        {
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    var listLeaves = context.Leaves
                        .Where(e => e.ApprovedBy == id)
                        .OrderByDescending(e => e.StartDate)
                        .Select(at => new LeaveDTO
                        {
                            LeaveId = at.LeaveId,
                            EmployeeId = at.EmployeeId,
                            LeaveTypeId = at.LeaveTypeId,
                            StartDate = at.StartDate,
                            EndDate = at.EndDate,
                            DurationInDays = at.DurationInDays,
                            Reason = at.Reason,
                            Status = at.Status,
                            ApprovedBy = at.ApprovedBy,
                            LeaveTypeName = at.LeaveType.LeaveTypeName,
                            ApprovedByName = at.Employee.EmployeeName,
                            EmployeeName = at.Employee.EmployeeName
                           
                        }).ToList();

                    return listLeaves;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error retrieving leave records: " + e.Message);
            }
        }

        // Create a new leave 
        public static bool AddLeave(LeaveDTO leaveDTO)
        {
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    var leave = new Leaf
                    {
                        ApprovedBy = leaveDTO.ApprovedBy,
                        ApprovedDate = DateTime.Now,
                        DurationInDays = leaveDTO.DurationInDays,
                        EmployeeId = leaveDTO.EmployeeId,
                        EndDate = leaveDTO.EndDate,
                        LeaveTypeId = leaveDTO.LeaveTypeId,
                        Reason = leaveDTO.Reason,
                        StartDate = leaveDTO.StartDate,
                        Status = leaveDTO.Status,
                        CreatedAt = DateTime.Now,
                        
                    };

                    context.Leaves.Add(leave);
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

        // Update an existing leave 
        public static bool UpdateLeave(LeaveDTO leaveDTO)
        {
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    var leave = context.Leaves.Find(leaveDTO.LeaveId);
                    if (leave == null || leave.DeletedAt != null)
                    {
                        return false; // Not found or already deleted
                    }

                    leave.Status = leaveDTO.Status;
                    leave.ApprovedBy = leaveDTO.ApprovedBy;
                    leave.ApprovedDate = DateTime.Now;

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                // Log exception if needed
                return false;
            }
        }


        // Soft delete a leave 
        public static bool DeleteLeave(int leaveID)
        {
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    var leave = context.Leaves.Find(leaveID);
                    if (leave == null || leave.DeletedAt != null)
                    {
                        return false; // Not found or already deleted
                    }

                    leave.DeletedAt = DateTime.Now;
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
