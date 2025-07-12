using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.EmployeeDTOS
{
    public class BookingOTDTO
    {
        public int OvertimeRequestId { get; set; }

        public int EmployeeId { get; set; }

        public DateOnly OvertimeDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }
        public string? Reason { get; set; }

        public string? Status { get; set; }
    }
}
