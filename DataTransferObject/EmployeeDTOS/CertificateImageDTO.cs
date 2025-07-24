using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.EmployeeDTOS
{
    public class CertificateImageDTO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeCertificateId { get; set; }

        [MaxLength(255)]
        public string ImageUrl { get; set; }

    }
}
