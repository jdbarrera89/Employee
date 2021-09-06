using Employee.Common.Models;
using Employee.Common.Responses;
using Employee.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

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

            if (string.IsNullOrEmpty(employeeTime?.IdEmployee.ToString()) ||
                string.IsNullOrEmpty(employeeTime?.Type.ToString()))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request must have IdEmployee and type."
                });
            }

            switch (employeeTime.Type)
            {
                case "0":
                    break;

                case "1":
                    break;

                default:
                    return new BadRequestObjectResult(new Response
                    {
                        IsSuccess = false,
                        Message = "The request must have IdEmployee and type."
                    });
            }

            EmployeeTimeEntity employeeTimeEntity = new EmployeeTimeEntity
            {
                IdEmployee = employeeTime.IdEmployee,
                ETag = "*",
                CreatedTime = DateTime.UtcNow,
                Type = employeeTime.Type,
                IsConsolidated = false,
                PartitionKey = "EmployeeTime",
                RowKey = Guid.NewGuid().ToString(),
            };

            TableOperation findOperation = TableOperation.Retrieve<EmployeeTimeEntity>("EmployeeTime", employeeTime.IdEmployee.ToString());
            TableResult findResult = await employeeTimeTable.ExecuteAsync(findOperation);
            EmployeeTimeEntity employee = (EmployeeTimeEntity)findResult.Result;

            if (findResult.Result)
            //if (employeeTime.Type.Equals("1")
            //{
            //    if (employeeTime)
            //}

            if (employeeTime.IdEmployee.ToString() == employeeTimeEntity.IdEmployee.ToString())
            {
                if (employeeTime.Type == "0" && !employeeTimeEntity.IsConsolidated)
                {
                    return new BadRequestObjectResult(new Response
                    {
                        IsSuccess = false,
                        Message = $"the employee already has a registered input."
                    });
                }
            }

            if (employeeTime.Type.Equals("0") && !employeeTimeEntity.IsConsolidated)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = $"the employee already has a registered input."
                });

            }         

            TableOperation addOperation = TableOperation.Insert(employeeTimeEntity);
            await employeeTimeTable.ExecuteAsync(addOperation);

            string message = "New employee time stored in table";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = employeeTimeEntity
            });
        }
    }
}
