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

    ///Ira
    public static class IraProductById
    {
        [FunctionName("IraProductById")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            int productID = -1;

            if (req.Method == "GET")
            { productID = int.Parse(req.Query["id"]); }

            if (req.Method == "POST")
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                productID = Convert.ToInt32(data.ProductID);
            }
            var connectionString = "Server=citizen.manukautech.info,6304;Database=CC2023_Group1_Project;UID=CC2023_Group1;PWD=fBit$65632;encrypt=true;trustservercertificate=true";
            log.LogInformation($"{connectionString}");
            try
            {
                Product product = null;
                using (SqlConnection c = new SqlConnection(connectionString))
                {
                    c.Open();
                    var query = "SELECT  ProductID,ProductName,ProductMaterial,ProductColour, ProductQty,ProductImg,ProductPrice, c.CategoryName, b.BrandName from Product as p join Category as c on c.CategoryID = p.CategoryID join Brand as b on b.BrandID = p.BrandID where ProductID =@pid"; //@pid is a placeholder we will use later in code for id
                    using (SqlCommand cmd = new SqlCommand(query, c))
                    {
                        cmd.Parameters.AddWithValue("@pid", productID);
                        using (SqlDataReader reader = cmd.ExecuteReader())// executes the query
                        {
                            while (reader.Read())   //it will read each line from the table we created 
                            {
                                product = new Product()
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

                            }
                        }
                    }

                    string json = System.Text.Json.JsonSerializer.Serialize(product);
                    return new OkObjectResult(json);
                }
            }
            catch (Exception ex) { return new BadRequestObjectResult(ex.Message); }

        }
    }
}
