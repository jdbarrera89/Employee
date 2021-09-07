using System;
using System.Threading.Tasks;
using Employee.Functions.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace Employee.Functions
{
    public static class ScheduledFunction
    {
        [FunctionName("ScheduledFunction")]
        public static async Task Run(
            [TimerTrigger("0 */2 * * * *")]TimerInfo myTimer,
            [Table("employeeTime", Connection = "AzureWebJobsStorage")] CloudTable employeeTimeTable,
            ILogger log)
        {
            log.LogInformation($"consolidated completed function executed at: {DateTime.Now}");
            string filter = TableQuery.GenerateFilterConditionForBool("IsConsolidated", QueryComparisons.Equal, true);
            TableQuery<EmployeeTimeEntity> query = new TableQuery<EmployeeTimeEntity>().Where(filter);
            TableQuerySegment<EmployeeTimeEntity> consolidatedTimes = await employeeTimeTable.ExecuteQuerySegmentedAsync(query, null);
            int consolidated = 0;

            for(int i=0; i <= consolidated; i++)
            {

            }
        }
    }
}
