using DataTransferObject.LeaveDTO;
using DataTransferObject.LeaveTypeDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.LeaveRepository
{
    public interface ILeaveRepository
    {
        List<LeaveDTO> GetLeaves();

        List<LeaveDTO> GetLeaveEmployee(int id);
        List<LeaveDTO> GetLeaveStaff(int id);
        bool AddLeave(LeaveDTO leaveDTO);
        bool UpdateLeave(LeaveDTO leaveDTO);
        bool DeleteLeave(int id);
    }
}
