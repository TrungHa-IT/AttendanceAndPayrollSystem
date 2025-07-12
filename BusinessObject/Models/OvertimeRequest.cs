using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class OvertimeRequest
{
    public int OvertimeRequestId { get; set; }

    public int EmployeeId { get; set; }

    public DateOnly OvertimeDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public decimal? TotalHours { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
    public bool? IsCheckIn { get; set; } 

    public virtual Employee Employee { get; set; } = null!;
}
