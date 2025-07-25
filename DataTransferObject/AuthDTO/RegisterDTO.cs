﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DataTransferObject.AuthDTO
{
    public class RegisterDTO
    {
        public int EmployId { get; set; }

        public string? EmployeeName { get; set; }
      
        public IFormFile? Image { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime? Dob { get; set; }

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Gender { get; set; }

        public string? Address { get; set; }

        public string? Position { get; set; }
        public decimal? Salary { get; set; }

        public int Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public int DepartmentId { get; set; }

        public string Password { get; set; } = null!;
    }
}
