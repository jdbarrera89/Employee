using System;

namespace Employee.Common.Models
{
    public class EmployeeTimeConsolidated
    {
        public int IdEmployee { get; set; }

        public DateTime CreatedTime { get; set; }

        public int minutesWorked { get; set; }
    }
}
