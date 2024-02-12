using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Functions.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;


namespace Functions
{
    public static class IraAllRentedProducts 
        //a simple function to display all rented products, Ira, 30/10/23
        //replacing IDs with client name, employee name and product name
    {
        [FunctionName("IraAllRentedProducts")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var connectionString = "Server=citizen.manukautech.info,6304;Database=CC2023_Group1_Project;UID=CC2023_Group1;PWD=fBit$65632;encrypt=true;trustservercertificate=true";
            log.LogInformation($"{connectionString}");
            try
            {
                List<RentedProduct> products = new List<RentedProduct>();
                using (SqlConnection c = new SqlConnection(connectionString))
                {
                    c.Open();           // opens connection
                    var query = @"Select RentID ,RentStartDate ,RentEndDate,RentQuantity ,
                                RentTotalPrice,pr.ProductImg, c.ClientName , pr.ProductName ,e.EmployeeName from RentedProduct as p
                                join Client as c on c.ClientID = p.ClientID
                                join Product as pr on pr.ProductID = p.ProductID
                                join Employee as e on e.EmployeeID = p.EmployeeID";
                    using (SqlCommand cmd = new SqlCommand(query, c))//link the query to the connection string
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader()) // executes the query 
                        {
                            while (reader.Read())   // read each line from the table we created
                            {
                                RentedProduct product = new RentedProduct()
                                {
                                    RentID = Convert.ToInt32(reader["RentID"]),
                                    RentStartDate = reader.GetDateTime(reader.GetOrdinal("RentStartDate")),
                                    RentEndDate = reader.GetDateTime(reader.GetOrdinal("RentEndDate")),
                                    RentQuantity = Convert.ToInt32(reader["RentQuantity"]),
                                    RentTotalPrice = Convert.ToDouble(reader["RentTotalPrice"]),
                                    ProductImg = reader["ProductImg"].ToString(),
                                    ProductName = reader["ProductName"].ToString(),
                                    ClientName = reader["ClientName"].ToString(),
                                    EmployeeName = reader["EmployeeName"].ToString(),
                                };
                                products.Add(product);
                            }
                        }
                    }

                }
                string json = System.Text.Json.JsonSerializer.Serialize(products); // convert code to a json string
                return new OkObjectResult(json);
            }
            catch (Exception ex) { return new BadRequestObjectResult(ex.Message); }

        }
    }
}
