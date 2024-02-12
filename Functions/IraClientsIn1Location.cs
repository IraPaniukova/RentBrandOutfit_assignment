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
//Ira, 12/10/2023, the functions allows to choose client from a specific location (LocationDescription)
{
    public static class IraClientsIn1Location
    {
        [FunctionName("IraClientsIn1Location")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string LocationDescription = null;
            if (req.Method == "GET")
            { LocationDescription = req.Query["LocationDescription"]; }
            if (req.Method == "POST")
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                LocationDescription = data.LocationDescription;
            }

            //adds the connection string
            var connectionString = "Server=citizen.manukautech.info,6304;Database=CC2023_Group1_Project;UID=CC2023_Group1;PWD=fBit$65632;encrypt=true;trustservercertificate=true";
            log.LogInformation($"{connectionString}");
            try
            {
                List<Client> clients = new List<Client>();
                using (SqlConnection c = new SqlConnection(connectionString))
                {
                    c.Open();           // opens connection
                    var query = "Select ClientID, ClientName, ClientEmail, ClientAddress, ClientPhone, l.LocationDescription from Client as c join Location as l on l.LocationID = c.LocationID where l.LocationDescription = @LocD"; //create a query
                    using (SqlCommand cmd = new SqlCommand(query, c))//links the query to the connection string
                    {
                        cmd.Parameters.AddWithValue("@LocD", LocationDescription);
                        using (SqlDataReader reader = cmd.ExecuteReader()) // executes the query 
                        {
                            while (reader.Read())   // reads each line from the table we created
                            {
                                Client cl = new Client()
                                //the function will fetch all columns from the table for the specific LocationDescription
                                {

                                    ClientID = Convert.ToInt32(reader["ClientID"]),
                                    ClientName = reader["ClientName"].ToString(),
                                    ClientEmail = reader["ClientEmail"].ToString(),
                                    ClientAddress = reader["ClientAddress"].ToString() ,
                                    ClientPhone = reader["ClientPhone"].ToString(),
                                    LocationDescription = reader["LocationDescription"].ToString()
                                };
                                clients.Add(cl);
                            }
                        }
                    }

                }
                string json = System.Text.Json.JsonSerializer.Serialize(clients); // convert code to a json string
                return new OkObjectResult(json);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
