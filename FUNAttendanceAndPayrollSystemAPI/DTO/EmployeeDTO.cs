namespace FUNAttendanceAndPayrollSystemAPI.DTO
{
    public class EmployeeDTO
    {
        public int EmployId { get; set; }

        public string? EmployeeName { get; set; }

        public DateOnly? Dob { get; set; }

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Gender { get; set; }

        public string? Address { get; set; }

        public string? Position { get; set; }

        public string Status { get; set; } = null!;
    }
}
