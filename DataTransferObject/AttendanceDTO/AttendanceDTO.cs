

namespace DataTransferObject.AttendanceDTO
{
    public class AttendanceDTO
    {
        public int AttendanceId { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public DateOnly WorkDate { get; set; }

        public TimeOnly CheckIn { get; set; }

        public TimeOnly? CheckOut { get; set; }
    }
}
