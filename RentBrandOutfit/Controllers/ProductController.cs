using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RentBrandOutfit.Models;
using System.Data;
using System.Net.Http.Headers;

namespace RentBrandOutfit.Controllers
{
    public class ProductController : Controller
    {

        private readonly IHttpClientFactory clientFactory;
        public ProductController(IHttpClientFactory cf) { clientFactory = cf; }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetToken()  //to get token function to get authentication for the API
        {
            try
            {
                var client = clientFactory.CreateClient();
                var url = "https://localhost:7242/api/Auth/login";
                var model = new Login
                {
                    username = "administrator@example.com",
                    password = "Password@123"

                };
                var response = client.PostAsJsonAsync(url, model).Result;  //ATTENTION - PostAsJsonAsync HERE
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result; //it wil return {"token" : "theToken Here"}
                    var jsonString = JObject.Parse(result); //parse the json string to the object
                    var token = jsonString["token"].ToString();     //get token out of the json string

                    TempData["AccessToken"] = token; //stire it in TempData to reuse in the next function


                    return Ok(token);  //just will show you a json string
                }
                else
                { return BadRequest(); }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        //there is no authorization to see all products, but there is uthentiction for the API Ira
        public IActionResult AllProducts() //Ira.24/10/23 //Ayako 1/11 chnaged function link to api
        {
            GetToken();
          
            try
            {   //adding authentication for API Ira, Ayako, 4/11/23
                var client = clientFactory.CreateClient();
                var url = "https://localhost:7242/api/APIProduct";
                if (TempData.ContainsKey("AccessToken"))  //AccessToken - var from the previous functon
                {
                    var token = TempData["AccessToken"] as string;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    //attaching the toke to the header of the request   

                    var response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        var products = JsonConvert.DeserializeObject<List<Product>>(result); // if responce is ok, it converts Json to list 
                        return View(products);
                    }
                }  
                return BadRequest(); 
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }


        [Authorize(Roles = " Administrator, Manager")]  //Ira, 4/11/2023 The finction to search by ID is only for admin or manager

        public IActionResult ProductSearchById(int id)  //Ira 24/10/23
        {
            var client = clientFactory.CreateClient();
            var functionURL = $"http://localhost:7031/api/IraProductById?id={id}";
            var response = client.GetAsync(functionURL).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                var pr = JsonConvert.DeserializeObject<ProductDetailed>(content); // convert json to object 
                return View(pr); // return updated view with object 
            }
            else
            {
                return BadRequest();
            }
        }


    }
}
