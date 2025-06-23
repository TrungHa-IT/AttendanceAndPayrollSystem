using DataTransferObject.DepartmentDTO;
using DataTransferObject.LeaveTypeDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.LeaveTypeRepository
{
    public interface ILeaveTypeRepository
    {
        List<LeaveTypeDTO> GetLeaveTypes();
        bool AddLeaveTypes(LeaveTypeDTO leaveTypeDTO);
        bool UpdateLeaveTypes(LeaveTypeDTO leaveTypeDTO);
        bool DeleteLeaveTypes(int id);
    }
}
