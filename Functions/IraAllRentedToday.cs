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
using System.Net.Http;
using Azure;

namespace Functions
{
    public static class IraAllRentedToday //The function is made to reuse other function, Ira, 2/11/2023
    {

        [FunctionName("IraAllRentedToday")]

        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var client = new HttpClient();
                var url = "http://localhost:7031/api/IraAllRentedProducts";
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var temp = JsonConvert.DeserializeObject<List<RentedProduct>>(result); // if responce is ok, it converts Json to list 
                    List<RentedProduct> products = new List<RentedProduct>();
                    foreach (var item in temp)
                    {
                        //var client2 = new HttpClient();  ////I TRIED TO REUSE THE SECOND FUNCTION,BUT IT IS VERY INEFFECTIVE, because it calls the function too many times and hold the progrem from executing
                        //var url2 = $"http://localhost:7031/api/IraIdRentedToday?id={item.RentID}";
                        //var response2 = client2.GetAsync(url2).Result;

                        //if (response2.IsSuccessStatusCode)
                        //{
                        //    var result2 = response2.Content.ReadAsStringAsync().Result;
                        //    var isRented = bool.Parse(result2);
                        //    if (isRented) { products.Add(item); }
                        //}
                        //else return new BadRequestObjectResult("An error occurred");

                        var connectionString = "Server=citizen.manukautech.info,6304;Database=CC2023_Group1_Project;UID=CC2023_Group1;PWD=fBit$65632;encrypt=true;trustservercertificate=true";
                        log.LogInformation($"{connectionString}");
                        try
                        {
                            int countN = 0;
                            using (SqlConnection c = new SqlConnection(connectionString))
                            {
                                c.Open();
                                var query = "SELECT count(RentID) as N from RentedProduct where RentID = @pid AND GETDATE() BETWEEN RentStartDate AND RentEndDate;"; //@pid is a placeholder we will use later in code for id
                               //I check the query for each Id from the list of rented products, if it is not null, add it to the list
                                
                                using (SqlCommand cmd = new SqlCommand(query, c))
                                {
                                    cmd.Parameters.AddWithValue("@pid", item.RentID);
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
                                    products.Add(item);
                                }

                            }
                        }
                        catch (Exception ex) { return new BadRequestObjectResult(ex.Message); }

                    }
                    string json = System.Text.Json.JsonSerializer.Serialize(products);
                    return new OkObjectResult(json);
                }
                else return new BadRequestObjectResult("An error occurred");
            }
            catch (Exception ex) { return new BadRequestObjectResult("An error occurred: " + ex.Message); }

        }
    }
}
