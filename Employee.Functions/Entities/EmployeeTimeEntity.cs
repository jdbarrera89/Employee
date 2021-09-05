using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Employee.Functions.Entities
{
    public class EmployeeTimeEntity : TableEntity
    {
        public int IdEmployee { get; set; }

        public DateTime CreatedTime { get; set; }

        public int Type { get; set; }

        public bool IsConsolidated { get; set; }
    }
}
