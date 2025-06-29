using DataAccess.LeaveTypeDAO;
using DataTransferObject.LeaveTypeDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.LeaveTypeRepository
{
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        public bool AddLeaveTypes(LeaveTypeDTO leaveTypeDTO) => LeaveTypeDAO.AddLeaveType(leaveTypeDTO);

        public bool DeleteLeaveTypes(int id) => LeaveTypeDAO.DeleteLeaveType(id);

        public List<LeaveTypeDTO> GetLeaveTypes() => LeaveTypeDAO.GetLeaveType();

        public bool UpdateLeaveTypes(LeaveTypeDTO leaveTypeDTO) => LeaveTypeDAO.UpdateLeaveType(leaveTypeDTO);
    }
}
