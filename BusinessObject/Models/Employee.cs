using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models;

public partial class Employee
{
    public int EmployId { get; set; }

    public string? EmployeeName { get; set; }

    [StringLength(500)]
    public string? Image { get; set; }

    public DateTime? Dob { get; set; }

    [StringLength(500)]
    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public string? Position { get; set; }
    public decimal Salary { get; set; }

    public int Status { get; set; } 

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int DepartmentId { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Leaf> Leaves { get; set; } = new List<Leaf>();

    public virtual ICollection<OvertimeRequest> OvertimeRequests { get; set; } = new List<OvertimeRequest>();

    public virtual ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    public virtual ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();
    public virtual ICollection<EmployeeCertificate> EmployeeCertificates { get; set; } = new List<EmployeeCertificate>();

}
