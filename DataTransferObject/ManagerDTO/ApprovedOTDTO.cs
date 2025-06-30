using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.ManagerDTO
{
    public class ApprovedOTDTO
    {
        public int OvertimeRequestId { get; set; }
        public DateOnly OvertimeDate { get; set; }
    }
}
