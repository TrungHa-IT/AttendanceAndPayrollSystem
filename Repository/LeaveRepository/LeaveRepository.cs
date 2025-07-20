using DataAccess.LeaveDAO;
using DataTransferObject.LeaveDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.LeaveRepository
{
    public class LeaveRepository : ILeaveRepository
    {
        public bool AddLeave(LeaveDTO leaveDTO) => LeaveDAO.AddLeave(leaveDTO);

        public bool DeleteLeave(int id) => LeaveDAO.DeleteLeave(id);

        public string GetEmailEmployee(int leaveId) => LeaveDAO.GetLeaveEmployeeEmail(leaveId);

        public List<LeaveDTO> GetLeaveEmployee(int id) => LeaveDAO.GetLeaveEmployee(id);

        public List<LeaveDTO> GetLeaves() => LeaveDAO.GetLeaves();

        public List<LeaveDTO> GetLeaveStaff(int id) => LeaveDAO.GetLeaveStaff(id);

        public bool UpdateLeave(LeaveDTO leaveDTO) => LeaveDAO.UpdateLeave(leaveDTO);
    }
}
