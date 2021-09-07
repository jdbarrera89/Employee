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

            EmployeeTimeEntity employeeTimeEntity = new EmployeeTimeEntity
            {
                IdEmployee = employeeTime.IdEmployee,
                ETag = "*",
                CreatedTime = DateTime.UtcNow,
                Type = employeeTime.Type,
                IsConsolidated = false,
                PartitionKey = "EmployeeTime",
                RowKey = Guid.NewGuid().ToString()
            };

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

        [FunctionName(nameof(UpdateTime))]
        public static async Task<IActionResult> UpdateTime(
           [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "employeeTime/{id}")] HttpRequest req,
           [Table("employeeTime", Connection = "AzureWebJobsStorage")] CloudTable employeeTimeTable,
           string id,
           ILogger log)
        {
            log.LogInformation($"Update for employeeTime: {id}, received.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            EmployeeTime employeeTime = JsonConvert.DeserializeObject<EmployeeTime>(requestBody);

            //Validate employee id
            TableOperation findOperation = TableOperation.Retrieve<EmployeeTimeEntity>("EmployeeTime", id);
            TableResult findResult = await employeeTimeTable.ExecuteAsync(findOperation);
            
            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "EmployeeTime not found."
                });
            }

            //Update employeeTime
            EmployeeTimeEntity employeeTimeEntity = (EmployeeTimeEntity)findResult.Result;
            employeeTimeEntity.IsConsolidated = employeeTime.IsConsolidated;
            if (!string.IsNullOrEmpty(employeeTime?.IdEmployee.ToString()))
            {
                employeeTimeEntity.IdEmployee = employeeTime.IdEmployee;
            }



            TableOperation addOperation = TableOperation.Replace(employeeTimeEntity);
            await employeeTimeTable.ExecuteAsync(addOperation);

            string message = $"employee time {id} updated in table";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = employeeTimeEntity
            });
        }

        [FunctionName(nameof(GetAllEmployeeTimes))]
        public static async Task<IActionResult> GetAllEmployeeTimes(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "employeeTime")] HttpRequest req,
        [Table("EmployeeTime", Connection = "AzureWebJobsStorage")] CloudTable employeeTimeTable,
        ILogger log)
        {
            log.LogInformation("Get all EmployeeTimes received.");

            TableQuery<EmployeeTimeEntity> query = new TableQuery<EmployeeTimeEntity>();
            TableQuerySegment<EmployeeTimeEntity> employeeTimes = await employeeTimeTable.ExecuteQuerySegmentedAsync(query, null);

            string message = "Retrieved all employeeTimes.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = employeeTimes
            });
        }

        [FunctionName(nameof(GetEmployeeTimeById))]
        public static IActionResult GetEmployeeTimeById(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "employeeTime/{id}")] HttpRequest req,
           [Table("employeeTime", "EmployeeTime", "{id}", Connection = "AzureWebJobsStorage")] EmployeeTimeEntity employeeTimeEntity,
           string id,
           ILogger log)
        {
            log.LogInformation($"Get employeeTime by id: {id}, received.");

            if (employeeTimeEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "EmployeeTime not found."
                });
            }

            string message = $"EmployeeTime: {employeeTimeEntity.RowKey}, retrieved.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = employeeTimeEntity
            });
        }

        [FunctionName(nameof(DeleteEmployeeTime))]
        public static async Task<IActionResult> DeleteEmployeeTime(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "employeeTime/{id}")] HttpRequest req,
            [Table("employeeTime", "EmployeeTime", "{id}", Connection = "AzureWebJobsStorage")] EmployeeTimeEntity employeeTimeEntity,
            [Table("employeeTime", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Delete employeeTime: {id}, received.");

            if (employeeTimeEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "EmployeeTime not found."
                });
            }

            await todoTable.ExecuteAsync(TableOperation.Delete(employeeTimeEntity));
            string message = $"Todo: {employeeTimeEntity.RowKey}, deleted.";
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
