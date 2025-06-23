using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.LeaveDTO
{
    public class LeaveDTO
    {
        public int LeaveId { get; set; }

        public int EmployeeId { get; set; }

        public int LeaveTypeId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public decimal? DurationInDays { get; set; }

        public string? Reason { get; set; }

        public string? Status { get; set; }

        public int? ApprovedBy { get; set; }

    }
}
