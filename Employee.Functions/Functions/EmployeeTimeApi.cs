using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using Employee.Common.Models;

namespace Employee.Functions.Functions
{
    public static class EmployeeTimeApi
    {
        [FunctionName(nameof(CreateInOut))]
        public static async Task<IActionResult> CreateInOut(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "employeeTime")] HttpRequest req,
            [Table("employeeTime", Connection = "AzureWebJobsStorage")] CloudTable employeeTimeTable,
            ILogger log)
        {
            log.LogInformation("Received a new employee time.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            EmployeeTime employeeTime = JsonConvert.DeserializeObject<EmployeeTime>(requestBody);   
            
            

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
