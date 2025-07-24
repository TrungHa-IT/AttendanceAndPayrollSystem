using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    public class EmployeeCertificate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CertificateName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        public string? Status { get; set; } // Bạn có thể dùng enum nếu muốn chuẩn hóa hơn

        public int? ApprovedBy { get; set; }

        public int? CertificateBonusRateId { get; set; }

        // ✅ Navigation properties
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; } = null!;

        [ForeignKey("CertificateBonusRateId")]
        public virtual CertificateBonusRate? CertificateBonusRate { get; set; }

        public virtual ICollection<EmployeeCertificateImage> Images { get; set; } = new List<EmployeeCertificateImage>();
    }
}
