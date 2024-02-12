using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RentBrandOutfit.Models;
using System.Data;

namespace RentBrandOutfit.Controllers
{
    [Authorize(Roles = " Administrator, Manager")]  //Ira, 4/11/2023 The whole controller is accessable only by admin or manager
    public class RentedProductController : Controller
    {
        private readonly IHttpClientFactory clientFactory;
        public RentedProductController(IHttpClientFactory cf) { clientFactory = cf; }
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AllRented() //Ira, 30/10/23
        {
            try
            {
                var client = clientFactory.CreateClient();
                var url = "http://localhost:7031/api/IraAllRentedProducts";
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var products = JsonConvert.DeserializeObject<List<RentedProductDetailed>>(result); // if responce is ok, it converts Json to list 
                    return View(products);
                }
                else
                { return BadRequest(); }
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        public IActionResult AllRentedToday() //Ira, 02/11/23  here I use a function that reuses another function
        {
            try
            {
                var client = clientFactory.CreateClient();
                var url = "http://localhost:7031/api/IraAllRentedToday";
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var products = JsonConvert.DeserializeObject<List<RentedProductDetailed>>(result); // if responce is ok, it converts Json to list 
                    return View(products);
                }
                else
                { return BadRequest(); }
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
     

        //Ira, 31/10/23, function to get total rent per category
        public IActionResult RentPerCategory()
        {
            try
            {
                var client = clientFactory.CreateClient();
                var url = "http://localhost:7031/api/IraRentPerCategory";
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var c = JsonConvert.DeserializeObject<List<CategoryRented>>(result); // if responce is ok, it converts Json to list 
                    return View(c);
                }
                else
                { return BadRequest(); }
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }



        //////Ayako 22/10/2023
        [HttpGet]
        public IActionResult showTop3RentedBrands()
        {
            try
            {
                var client = clientFactory.CreateClient();
                var url = "http://localhost:7031/api/AyakoTop3RentedBrand";
                var responce = client.GetAsync(url).Result;

                if (responce.IsSuccessStatusCode)
                {
                    var content = responce.Content.ReadAsStringAsync().Result;

                    if (!string.IsNullOrEmpty(content) && content != "null") // checking if list is not empty 
                    {
                        var Ob = JsonConvert.DeserializeObject<List<BrandandTotalPrice>>(content); // if not, covert json to list of brand with total sold price 
                        View(Ob);
                    }
                    else
                    {
                        ViewBag.Error = "nothing to get"; // if the list is empty, store string to view bag 
                    }
                }
                else
                {
                    ViewBag.Error = responce.StatusCode + "Error"; // if bad request, it stores error message in view bag 
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message; // if exception is thrown, it stores in view bag 
            }
            return View();
        }


        [HttpPost]
        //Ayako 31/10/2023 
        public IActionResult rentProduct(DateTime rentStart, DateTime rentEnd, int QTY, int clientID, int productID, int empID) {

            //example http://localhost:7031/api/AyakoRentProduct?rentStart=2023-11-01&rentEnd=2023-11-05&QTY=2&clientID=1&productID=3&empID=2
            try
            {
                var messanger = this.clientFactory.CreateClient();
                string address = $"http://localhost:7031/api/AyakoRentProduct";


                //post method should be used as it contains sensitive information 

                

                var requestData = new
                {
                    rentStart = rentStart.Date.ToString("yyyy-MM-dd"),
                    rentEnd = rentEnd.Date.ToString("yyyy-MM-dd"),
                    QTY = QTY,
                    clientID = clientID,
                    productID = productID,
                    empID = empID

                };

               
                //converting it to jason string
                var content = new StringContent(JsonConvert.SerializeObject(requestData), System.Text.Encoding.UTF8, "application/json");
                //call axire finction (send post request) this is post version
                var responce = messanger.PostAsync(address, content).Result;
                if (responce.IsSuccessStatusCode)
                {
                    var result = responce.Content.ReadAsStringAsync().Result;

                    if (!string.IsNullOrEmpty(result) && result != "null")
                    {

                        // ViewBag.Result = result;
                        RentResponce OB = JsonConvert.DeserializeObject<RentResponce>(result);
                        return View(OB);
                        // return Ok(OB);
                    }

                    else
                    {

                        ViewBag.Er = "rent failed";

                    }

                }

                else
                {
                    
                    ViewBag.Er = responce.Content.ReadAsStringAsync().Result;
                    // return   StatusCode((int)responce.StatusCode, "Error");
                }

                return View();
            }
            catch (Exception ex)
            {

                ViewBag.Er = ex.Message; // if exceotion thrown, it assign error message to view bag 
                return View();
            }




        }

        [HttpGet]
        public IActionResult rentProduct() {

            return View();
        }

    }
}
