using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DateTimeDTO
{
    public class WeekOption
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Display => $"{StartDate:dd/MM} to {EndDate:dd/MM}";
    }
}
