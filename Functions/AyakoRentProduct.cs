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
using System.Runtime.CompilerServices;
using System.Net.Http;
using Azure;
using System.ComponentModel.Design;

namespace Functions
{
    //Ayako 30/10/2023
    public static class AyakoRentProduct
    {
        [FunctionName("AyakoRentProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {


            try
            {
                log.LogInformation("C# HTTP trigger function processed a request.");

               

                DateTime rentStart;
                DateTime rentEnd;
                int QTY;
                decimal total;
                int clientID;
                int productID;
                int empID;





                if (req.Method == "GET")
                {

                    //get dates as string first
                    string rentS = req.Query["rentStart"];
                    string rentE = req.Query["rentEnd"];
                    QTY = int.Parse(req.Query["QTY"]);
                    clientID = int.Parse(req.Query["clientID"]);
                    productID = int.Parse(req.Query["productID"]);
                    empID = int.Parse(req.Query["empID"]);
                    // DateTime.TryParse tries to parse string rentS to DateTime dateStart  //https://learn.microsoft.com/en-us/dotnet/api/system.datetime.tryparse?view=net-7.0
                    if (!DateTime.TryParse(rentS, out rentStart))
                    {
                        throw new Exception("Invalid date format for start date");
                    }

                    if (!DateTime.TryParse(rentE, out rentEnd))
                    {
                        throw new Exception("Invalid date format for end date");
                    }



                }

                else if (req.Method == "POST")
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    dynamic data = JsonConvert.DeserializeObject(requestBody);


                    string rentS = data.rentStart;
                    string rentE = data.rentEnd;
                    QTY = Convert.ToInt32(data.QTY);
                    clientID = Convert.ToInt32(data.clientID);
                    productID = Convert.ToInt32(data.productID);
                    empID = Convert.ToInt32(data.empID);

                    //parse to datetime type when you recieve by Post method too

                    if (!DateTime.TryParse((string)data.rentStart, out rentStart))
                    {
                       throw new Exception("Invalid date format for start date");
                    }

                    if (!DateTime.TryParse((string)data.rentEnd, out rentEnd))
                    {
                        throw new Exception("Invalid date format for end date");
                    }

                }

                else
                {
                    throw new Exception("Please choose  get or port");
                }

                // if rent start date is more than end date, it throws exception, abd also if rent start date is before today
                if (rentStart > rentEnd || rentStart < DateTime.Today)
                {
                    throw new Exception("please choose correct dates, rent start date connot be before today or rend end date can not be before start date.");
                }
                else
                {
                    //Calculating how many days //https://www.bytehide.com/blog/timespan-csharp 
                    TimeSpan duration = rentEnd - rentStart;
                    int days = duration.Days;


                    //calling function search client and product 
                    var client = new HttpClient();
                    var urlC = $"http://localhost:7031/api/AyakoClientSearchByID?id={clientID}";
                    var responceC = client.GetAsync(urlC).Result;
                    var urlP = $"http://localhost:7031/api/IraProductById?id={productID}";
                    var responceP = client.GetAsync(urlP).Result;

                    if (responceC.IsSuccessStatusCode && responceP.IsSuccessStatusCode)
                    {
                        var resultC = responceC.Content.ReadAsStringAsync().Result;
                        Client c = JsonConvert.DeserializeObject<Client>(resultC);
                        var resultP = responceP.Content.ReadAsStringAsync().Result;
                        Product p = JsonConvert.DeserializeObject<Product>(resultP);


                        // This is my first try, I called functions in the same file here 

                        /*  Product p = await GetProductById(productID, log);
                          Client c = await GetClientById(clientID, log);*/

                        //if product is null returns bad request 
                        if (p == null) { return new BadRequestObjectResult("could not fetch product information"); }

                        //if client is null returns bad request 
                        if (c == null) { return new BadRequestObjectResult("could not fetch client information"); }


                        // getting product price
                        decimal productPrice = (decimal)p.ProductPrice;
                        // calculating total 
                        //  total = QTY * (rentStart - rentEnd)*unitprice; 
                        total = QTY * days * productPrice;

                        var connection = "Server=citizen.manukautech.info,6304;Database=CC2023_Group1_Project;UID=CC2023_Group1;PWD=fBit$65632;encrypt=true;trustservercertificate=true"; // our database connection string 

                        using (SqlConnection con = new SqlConnection(connection))
                        {


                            con.Open(); // open connection 
                            Record record = new Record(); // list for records

                            //inserting record 
                            string query = "INSERT INTO RentedProduct (RentStartDate, RentEndDate, RentQuantity, RentTotalPrice, ClientID, ProductID, EmployeeID) " +
                            "VALUES (@RentStartDate, @RentEndDate, @RentQuantity, @RentTotalPrice, @ClientID, @ProductID, @EmployeeID)";

                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {

                                cmd.Parameters.AddWithValue("@RentStartDate", rentStart);
                                cmd.Parameters.AddWithValue("@RentEndDate", rentEnd);
                                cmd.Parameters.AddWithValue("@RentQuantity", QTY);

                                cmd.Parameters.AddWithValue("@RentTotalPrice", total);
                                cmd.Parameters.AddWithValue("@ClientID", clientID);
                                cmd.Parameters.AddWithValue("@ProductID", productID);
                                cmd.Parameters.AddWithValue("@EmployeeID", empID);

                                cmd.ExecuteNonQuery();



                            }

                            record.RentStartDate = rentStart;
                            record.RentEndDate = rentEnd;
                            record.ClientID = clientID;
                            record.ProductID = productID;
                            record.EmployeeID = empID;

                            // Creating responce object 

                            var response = new
                            {
                                RentStartDate = record.RentStartDate,
                                RentEndDate = record.RentEndDate,
                                ProductID = p.ProductID,
                                ProductName = p.ProductName,
                                ProductImg = p.ProductImg,
                                ClientName = c.ClientName,
                                ClientID = c.ClientID,
                                total = total,
                                days = days,
                                QTY = QTY,
                                price = p.ProductPrice


                            };


                            string Json = JsonConvert.SerializeObject(response);

                            return new OkObjectResult(response);
                        }








                    }
                    else {

                        throw new Exception("could not call function search product or search client");
                    }
                }
            }catch(Exception ex) { return new BadRequestObjectResult(ex.Message);}



            

        }


        // those are functions I called for getting client and product information,but now replaced by calling fucnctions by HttpClient //https://auth0.com/blog/introduction-to-async-programming-in-csharp/ // https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-scenarios

        public static async Task<Product> GetProductById(int productID, ILogger log)
        {

            //I just copied and pasted from Ira's getProductByID function 
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

                    return product;
                }
            }
            catch (Exception ex) { log.LogInformation(ex.Message) ;
                return null;
            }

        }

        public static async Task<Client> GetClientById(int ClientID, ILogger log)
        {
          //I copied and pasted my getClientByID function 
            try
            {///try and catch 

              


                var connectionString = "Server=citizen.manukautech.info,6304;Database=CC2023_Group1_Project;UID=CC2023_Group1;PWD=fBit$65632;encrypt=true;trustservercertificate=true"; // connection strgin to our database 

                log.LogInformation($"ConnectionSrting: {connectionString}");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    Client clinet = null;
                    connection.Open();

                    string query = "select ClientID,ClientName,ClientEmail, ClientAddress, ClientPhone FROM Client WHERE ClientID = @id";



                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@id", ClientID);

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


                    return clinet;

                }







            }
            catch (Exception ex)
            {

                log.LogInformation(ex.Message);
                return null;
            }

        }
    }

    }

