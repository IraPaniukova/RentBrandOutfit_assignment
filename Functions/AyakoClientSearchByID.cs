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

    //Ayako 13/10/2023

    public static class AyakoClientSearchByID
    {
        [FunctionName("AyakoClientSearchByID")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            int id; // input is int id

            try
            {///try and catch 

                if (req.Method == "GET")
                {
                    id = int.Parse(req.Query["id"]);
                }

                else if (req.Method == "POST")
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    dynamic data = JsonConvert.DeserializeObject(requestBody);
                    id = Convert.ToInt32(data.id);


                }

                else
                {

                    throw new Exception("Please choose get or post"); // if non of them is returned then it throws exception 


                }


                var connectionString = "Server=citizen.manukautech.info,6304;Database=CC2023_Group1_Project;UID=CC2023_Group1;PWD=fBit$65632;encrypt=true;trustservercertificate=true"; // connection strgin to our database 

                log.LogInformation($"ConnectionSrting: {connectionString}");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    Client clinet = null;
                    connection.Open();

                    string query = "select ClientID,ClientName,ClientEmail, ClientAddress, ClientPhone FROM Client WHERE ClientID = @id";



                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read()) // loop eadch record and create object 
                            {
                                clinet = new Client
                                {
                                    ClientID = Convert.ToInt32(reader["ClientID"]),
                                    ClientName = reader["ClientName"].ToString(),
                                    ClientEmail = reader["ClientEmail"].ToString(),
                                    ClientAddress = reader["ClientAddress"].ToString(),
                                    ClientPhone = reader["ClientPhone"].ToString()





                                };



                            }









                        }







                    }


                    string Json = System.Text.Json.JsonSerializer.Serialize(clinet); // convert to Json string 
                    return new OkObjectResult(Json); // return to controller 

                }







            }
            catch (Exception ex)
            {

                return new BadRequestObjectResult(ex.Message); // if catches exception it will return string message to controller 
            }


        }








    }
}
