using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int EmployeeId { get; set; }

    public DateOnly WorkDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
