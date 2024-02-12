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
    //Ira, 23/1023 It is a simple function that is displayinh All clients from our database
    public static class IraAllClients
    {
        [FunctionName("IraAllClients")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var connectionString = "Server=citizen.manukautech.info,6304;Database=CC2023_Group1_Project;UID=CC2023_Group1;PWD=fBit$65632;encrypt=true;trustservercertificate=true";
            log.LogInformation($"{connectionString}");
            try
            {
                List<Client> clients = new List<Client>();
                using (SqlConnection c = new SqlConnection(connectionString))
                {
                    c.Open();           // opens connection
                    var query = "SELECT ClientID,ClientName,ClientEmail,ClientAddress,ClientPhone , l.LocationDescription from Client as c join Location as l on l.LocationID = c.LocationID";
                    using (SqlCommand cmd = new SqlCommand(query, c))//links the query to the connection string
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader()) // executes the query 
                        {
                            while (reader.Read())   // read each line from the table we created
                            {
                                Client client = new Client()
                                //I am choosing to use all our DB columns from the table:
                                {
                                    ClientID = Convert.ToInt32(reader["ClientID"]),
                                    ClientName = reader["ClientName"].ToString(),
                                    ClientEmail = reader["ClientEmail"].ToString(),
                                    ClientAddress = reader["ClientAddress"].ToString(),
                                    ClientPhone = reader["ClientPhone"].ToString(),
                                    LocationDescription = reader["LocationDescription"].ToString()
                                };
                                clients.Add(client);
                            }
                        }
                    }

                }
                string json = System.Text.Json.JsonSerializer.Serialize(clients); // convert code to a json string
                return new OkObjectResult(json);
            }
            catch (Exception ex) { return new BadRequestObjectResult(ex.Message); }


        }
    }
}
