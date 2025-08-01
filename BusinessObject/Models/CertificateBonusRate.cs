﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    public class CertificateBonusRate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string CertificateName { get; set; }

        [Required]
        public decimal BonusAmount { get; set; } 
        public ICollection<EmployeeCertificate> EmployeeCertificates { get; set; }
    }
}
