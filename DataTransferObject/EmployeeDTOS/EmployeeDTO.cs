﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.EmployeeDTOS
{
    public class EmployeeDTO
    {
        public int EmployId { get; set; }

        public string? EmployeeName { get; set; }
        public string? Image { get; set; }

        public DateTime? Dob { get; set; }

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Gender { get; set; }

        public string? Address { get; set; }
        public DateTime CreateAt { get; set; }
        public string? Position { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public decimal? Salary { get; set; }
        public int Status { get; set; }
        public double PayrollTime { get; set; }
        public double OvertimeTime { get; set; }
        public double TotalTimeWorked { get; set; }
        public decimal TotalSalary { get; set; }

    }
}
