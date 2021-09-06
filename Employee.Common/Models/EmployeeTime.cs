using System;

namespace Employee.Common.Models
{
    public class EmployeeTime
    {
        public int IdEmployee { get; set; }

        public DateTime CreatedTime { get; set; }

        public string Type { get; set; }

        public bool IsConsolidated { get; set; }
    }
}