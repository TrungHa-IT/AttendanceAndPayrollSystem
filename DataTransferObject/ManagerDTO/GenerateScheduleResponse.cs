﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.ManagerDTO
{
    public class GenerateScheduleResponse
    {
        public string Message { get; set; }
        public List<string> Skipped { get; set; }
    }
}
