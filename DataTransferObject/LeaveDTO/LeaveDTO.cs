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

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal? DurationInDays { get; set; }

        public string? Reason { get; set; }

        public string? Status { get; set; }

        public int? ApprovedBy { get; set; }

        public string? LeaveTypeName { get; set; }
        public string? ApprovedByName { get; set; }
        public string? EmployeeName { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
