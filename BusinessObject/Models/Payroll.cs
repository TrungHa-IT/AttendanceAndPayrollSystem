using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Payroll
{
    public int PayrollId { get; set; }

    public int EmployeeId { get; set; }

    public int Month { get; set; }

    public int? Year { get; set; }

    public decimal? TotalWorkHour { get; set; }

    public decimal? TotalSalary { get; set; }

    public decimal? HourlyRate { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
