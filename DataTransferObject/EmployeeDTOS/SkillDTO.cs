using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.EmployeeDTOS
{
    public class SkillDTO
    {
        public int SkillId { get; set; }
        public int EmployId { get; set; }
        public string SkillName { get; set; }
        public string Level { get; set; }
    }
}
