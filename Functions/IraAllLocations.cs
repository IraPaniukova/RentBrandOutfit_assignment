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
using System.Collections.Generic;
using Functions.Models;

namespace Functions //Ira, 12/10/2023, In this function I fetch all our location descriptions
{
    public static class IraAllLocations 
    {
        [FunctionName("IraAllLocations")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //add the connection string
            var connectionString = "Server=citizen.manukautech.info,6304;Database=CC2023_Group1_Project;UID=CC2023_Group1;PWD=fBit$65632;encrypt=true;trustservercertificate=true";
            log.LogInformation($"{connectionString}");
            try
            {
                List<Location> locations = new List<Location>();
                using (SqlConnection c = new SqlConnection(connectionString))
                {
                    c.Open();           // opens connection
                    var query = "Select LocationDescription from Location";//create a query
                    using (SqlCommand cmd = new SqlCommand(query, c))//link the query to the connection string
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader()) // executes the query 
                        {
                            while (reader.Read())   // reads each line from the table we created
                            {
                                Location loc = new Location()
                                {
                                    LocationDescription = reader["LocationDescription"].ToString(),                                    
                                };
                                locations.Add(loc);
                            }
                        }
                    }

                }
                string json = System.Text.Json.JsonSerializer.Serialize(locations); // convert code to a json string
                return new OkObjectResult(json);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
