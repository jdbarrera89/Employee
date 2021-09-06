using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Employee.Functions.Entities
{
    public class EmployeeTimeEntity : TableEntity
    {
        public int IdEmployee { get; set; }

        public DateTime CreatedTime { get; set; }

        public string Type { get; set; }

        public bool IsConsolidated { get; set; }
    }
}
