using System;
using System.Collections.Generic;

namespace BusinessObject.Models;

public partial class Employee
{
    public int EmployId { get; set; }

    public string? EmployeeName { get; set; }

    public DateOnly? Dob { get; set; }

    public int? AccountId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
