using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Attendance
{
    public int AttendanceId { get; set; }

    public int EmployeeId { get; set; }

    public DateOnly WorkDate { get; set; }

    public TimeOnly CheckIn { get; set; }

    public TimeOnly? CheckOut { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
