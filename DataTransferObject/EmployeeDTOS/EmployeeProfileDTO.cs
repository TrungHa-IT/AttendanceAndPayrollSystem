using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.EmployeeDTOS
{
        public class EmployeeProfileDTO
        {
            public int EmployId { get; set; }
            public string? EmployeeName { get; set; }
            public string? Image { get; set; }
            public DateOnly? Dob { get; set; }
            public string Email { get; set; } = null!;
            public string? PhoneNumber { get; set; }
            public string? Gender { get; set; }
            public string? Address { get; set; }
            public string? Position { get; set; }

            public List<SkillDTO> Skills { get; set; } = new();
            public List<CertificateDTO> Certificates { get; set; } = new();
        }

    public class UpdateSkillsRequest
    {
        public int EmployeeId { get; set; }
        public List<SkillDTO> Skills { get; set; }
    }

    public class UpdateCertificatesRequest
    {
        public int EmployeeId { get; set; }
        public List<CertificateDTO> Certificates { get; set; }
    }

}
