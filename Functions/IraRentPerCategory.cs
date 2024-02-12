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

namespace Functions
{
    public static class IraRentPerCategory  //RentTotalPrice for each category, Ira 30/10/23
    {
        [FunctionName("IraRentPerCategory")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var connectionString = "Server=citizen.manukautech.info,6304;Database=CC2023_Group1_Project;UID=CC2023_Group1;PWD=fBit$65632;encrypt=true;trustservercertificate=true";
            log.LogInformation($"{connectionString}");
            try
            {
                List<RentPerCategory> totals = new List<RentPerCategory>();
                using (SqlConnection c = new SqlConnection(connectionString))
                {
                    c.Open();           // opens connection
                    var query = @"Select sum(RentTotalPrice) as ""TotalPerCategory"", CategoryName from RentedProduct as p
                        join Product as pr on pr.ProductID = p.ProductID
                        join Category as c on c.CategoryID =pr.CategoryID
                        group by CategoryName 
                        ORDER BY SUM(RentTotalPrice) DESC";
                    using (SqlCommand cmd = new SqlCommand(query, c))//link the query to the connection string
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader()) // executes the query 
                        {
                            while (reader.Read())   // read each line from the table we created
                            {
                                RentPerCategory total = new RentPerCategory()
                                {
                                    CategoryName = reader["CategoryName"].ToString(),

                                    TotalPerCategory = Convert.ToDouble(reader["TotalPerCategory"]),
                                   
                                };
                                totals.Add(total);
                            }
                        }
                    }

                }
                string json = System.Text.Json.JsonSerializer.Serialize(totals); // convert code to a json string
                return new OkObjectResult(json);
            }
            catch (Exception ex) { return new BadRequestObjectResult(ex.Message); }

        }
    }
}
