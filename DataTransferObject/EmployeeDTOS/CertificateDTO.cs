using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace DataTransferObject.EmployeeDTOS
{
    public class CertificateDTO
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string CertificateName { get; set; } = string.Empty;

        public DateTime IssueDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string? Status { get; set; }
        public int? ApprovedBy { get; set; }

        public string? ApprovedByName { get; set; }

        public int? CertificateBonusRateId { get; set; }

        public decimal? BonusRate { get; set; }

        public string? EmployeeName { get; set; }

        public List<IFormFile> ImageFiles { get; set; } = new(); // file upload từ form
        public List<string> ImageUrls { get; set; } = new();
    }
}
