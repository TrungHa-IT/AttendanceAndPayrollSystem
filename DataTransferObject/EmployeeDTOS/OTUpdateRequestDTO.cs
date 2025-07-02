using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.EmployeeDTOS
{
    public class OTUpdateRequestDTO
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public string Status { get; set; }
    }
}
