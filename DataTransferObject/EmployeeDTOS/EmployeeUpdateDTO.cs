using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.EmployeeDTOS
{
    public class EmployeeUpdateDTO
    {
        public int EmployId { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime? Dob { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? Position { get; set; }
        public decimal Salary { get; set; }
        public int Status { get; set; }
        public int DepartmentId { get; set; }
        public string? Image { get; set; }
    }
}
