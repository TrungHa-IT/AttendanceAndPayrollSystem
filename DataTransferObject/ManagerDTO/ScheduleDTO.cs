using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.ManagerDTO
{
    public class ScheduleDTO
    {
        public int ScheduleId { get; set; }

        public int EmployeeId { get; set; }

        public DateTime WorkDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }
    }
}
