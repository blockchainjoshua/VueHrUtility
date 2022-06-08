using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VueHrUtilityV1.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public string EmployeeId { get; set; }
        public string Source { get; set; }
        public bool? IsEdited { get; set; }
        public string EditedBy { get; set; }
    }
}
