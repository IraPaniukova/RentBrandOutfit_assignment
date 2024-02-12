using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Functions.Models;

namespace Functions
{


    public static class IraAllProducts //creatung a new function. Ira 22/09/23; Modified on 23/10/23 - replaced foreign IDs with its names
    {
        [FunctionName("IraAllProducts")]

        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var connectionString = "Server=citizen.manukautech.info,6304;Database=CC2023_Group1_Project;UID=CC2023_Group1;PWD=fBit$65632;encrypt=true;trustservercertificate=true";
            log.LogInformation($"{connectionString}");
            try
            {
                List<Product> products = new List<Product>();
                using (SqlConnection c = new SqlConnection(connectionString))
                {
                    c.Open();           // opens connection
                    var query = "SELECT ProductID,ProductName,ProductMaterial,ProductColour, ProductQty,ProductImg,ProductPrice, c.CategoryName, b.BrandName from Product as p join Category as c on c.CategoryID = p.CategoryID join Brand as b on b.BrandID = p.BrandID";
                    using (SqlCommand cmd = new SqlCommand(query, c))//link the query to the connection string
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader()) // executes the query 
                        {
                            while (reader.Read())   // read each line from the table we created
                            {
                                Product product = new Product()
                                {
                                    ProductID = Convert.ToInt32(reader["ProductID"]),
                                    ProductName = reader["ProductName"].ToString(),
                                    ProductMaterial = reader["ProductMaterial"].ToString(),
                                    ProductColour = reader["ProductColour"].ToString(),
                                    ProductQty = Convert.ToInt32(reader["ProductQty"]),
                                    ProductImg = reader["ProductImg"].ToString(),
                                    ProductPrice = Convert.ToDouble(reader["ProductPrice"]),
                                    CategoryName = reader["CategoryName"].ToString(),
                                    BrandName = reader["BrandName"].ToString()
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
