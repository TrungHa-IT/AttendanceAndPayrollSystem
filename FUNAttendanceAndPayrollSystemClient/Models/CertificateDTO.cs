using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNAttendanceAndPayrollSystemClient.EmployeeDTOS
{
    public class CertificateDTO
    {
        public int CertificateId { get; set; }
        public int EmployeeId { get; set; }
        public string CertificateName { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
