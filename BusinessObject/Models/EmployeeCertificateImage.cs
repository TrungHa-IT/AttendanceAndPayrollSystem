using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObject.Models
{
    public class EmployeeCertificateImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeCertificateId { get; set; }

        [MaxLength(255)]
        public string ImageUrl { get; set; }

        [ForeignKey("EmployeeCertificateId")]
        public EmployeeCertificate EmployeeCertificate { get; set; }
    }
}
