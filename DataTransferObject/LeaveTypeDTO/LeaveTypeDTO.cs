﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.LeaveTypeDTO
{
    public class LeaveTypeDTO
    {
        public int LeaveTypeId { get; set; }

        public string LeaveTypeName { get; set; } = null!;

        public decimal? MaxDaysAllowed { get; set; }

        public bool IsPaid { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
