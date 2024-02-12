using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using Functions.Models;

namespace Functions
{
    public static class IraIdRentedToday //the function returns true if an Item by ID is rented today, Ira, 1/11/2023
    {
        [FunctionName("IraIdRentedToday")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            int rentID = -1;

            if (req.Method == "GET")
            { rentID = int.Parse(req.Query["id"]); }

            if (req.Method == "POST")
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                rentID = Convert.ToInt32(data.RentID);
            }
            var connectionString = "Server=citizen.manukautech.info,6304;Database=CC2023_Group1_Project;UID=CC2023_Group1;PWD=fBit$65632;encrypt=true;trustservercertificate=true";
            log.LogInformation($"{connectionString}");
            try
            {
                int countN = 0;
                using (SqlConnection c = new SqlConnection(connectionString))
                {
                    c.Open();
                    var query = "SELECT count(RentID) as N from RentedProduct where ProductID = @pid AND GETDATE() BETWEEN RentStartDate AND RentEndDate;"; //@pid is a placeholder we will use later in code for id
                    using (SqlCommand cmd = new SqlCommand(query, c))
                    {
                        cmd.Parameters.AddWithValue("@pid", rentID);
                        using (SqlDataReader reader = cmd.ExecuteReader())// executes the query
                        {
                            while (reader.Read())   
                            {
                                 countN = Convert.ToInt32(reader["N"]);
                            }
                        }
                    }
                    if (countN != 0)
                    {
                        return new OkObjectResult(true);
                    }
                    else { return new OkObjectResult(false); }
                    
                }
            }
            catch (Exception ex) { return new BadRequestObjectResult(ex.Message); }

        }
    }
}
