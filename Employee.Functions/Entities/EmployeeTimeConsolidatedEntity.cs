using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Employee.Functions.Entities
{
    internal class EmployeeTimeConsolidatedEntity : TableEntity
    {
        public int IdEmployee { get; set; }

        public DateTime CreatedTime { get; set; }

        public int minutesWorked { get; set; }
    }
}
