using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RentBrandOutfit.Models;
using System.Data;
using System.Reflection.Metadata.Ecma335;

namespace RentBrandOutfit.Controllers
{
    [Authorize(Roles = " Administrator, Manager")]  //Ira, 4/11/2023 The whole controller is accessable only by admin or manager

    public class ClientController : Controller
    {

        private readonly IHttpClientFactory clientFactory;
        public ClientController(IHttpClientFactory cf) { clientFactory = cf; }


        public IActionResult Index()
        {
            return View();     // we keep it as a plain view because we use it for displaying buttons to acces another actions
        }

        public IActionResult AllClients() //Ira.24/10/23
        {
            try
            {
                var client = clientFactory.CreateClient();
                var url = "http://localhost:7031/api/IraAllClients";
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var clients = JsonConvert.DeserializeObject<List<ClientDetailed>>(result);
                    return View(clients);
                }
                else
                { return BadRequest(); }
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }


        public IActionResult ClientsInLocation(string selectedItem)  //Ira 12/10/23, 14/10/23, 16/10/23
                                                                     //I spent more time here because originaly the button and choice for the list of location was in the Index view, but I wanted it to be in here
                                                                     //I researched how to do it on my own
        {

            try
            {
                if (string.IsNullOrEmpty(selectedItem))
                {
                    // initial GET request
                    //I use variables with one 1 at the end for locations 
                    var clientl = clientFactory.CreateClient();
                    var functionURLl = "http://localhost:7031/api/IraAllLocations";
                    var responsel = clientl.GetAsync(functionURLl).Result;
                    if (responsel.IsSuccessStatusCode)
                    {
                        var contentl = responsel.Content.ReadAsStringAsync().Result;
                        var locations = JsonConvert.DeserializeObject<List<Location>>(contentl);

                        var cl = new ClientLocation
                        {
                            Locations = locations,
                            Clients = new List<ClientDetailed>() // Clients as an empty list
                        };

                        return View(cl);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    //  POST request with selectedItem
                    //I use variables with one 1 at the end for locations and without - for clients
                    var clientl = clientFactory.CreateClient();
                    var functionURLl = "http://localhost:7031/api/IraAllLocations";
                    var client = clientFactory.CreateClient();
                    var functionURL = $"http://localhost:7031/api/IraClientsIn1Location?LocationDescription={selectedItem}";
                    var responsel = clientl.GetAsync(functionURLl).Result;
                    var response = client.GetAsync(functionURL).Result;
                    ViewBag.Location = selectedItem;

                    if (responsel.IsSuccessStatusCode && response.IsSuccessStatusCode)
                    {
                        var contentl = responsel.Content.ReadAsStringAsync().Result;
                        var content = response.Content.ReadAsStringAsync().Result;
                        var locations = JsonConvert.DeserializeObject<List<Location>>(contentl);  //list of locations
                        var clients = JsonConvert.DeserializeObject<List<ClientDetailed>>(content); //list of clients
                        ClientLocation cl = new ClientLocation
                        {
                            Locations = locations,
                            Clients = clients
                        };

                        return View(cl); //sending lists locations and clients to the view
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }





        public IActionResult ClientSearchByID() //Ayako 13/10/23
        {
            return View();


        }
        [HttpPost]
        public IActionResult ClientSearchByID(int id) //Ayako 13/10/23 //Ayako Edited 21/10/2023
        {
            try
            {
                var messanger = this.clientFactory.CreateClient();
                string address = $"http://localhost:7031/api/AyakoClientSearchByID?id={id}";

                var responce = messanger.GetAsync(address).Result;

                if (responce.IsSuccessStatusCode)
                {

                    var result = responce.Content.ReadAsStringAsync().Result;

                    if (!string.IsNullOrEmpty(result) && result != "null") // checking if client is not empty 
                    {
                        var OB = JsonConvert.DeserializeObject<ClientDetailed>(result); // if it's not empty, covert to object 

                        return View(OB); // return updated view with object if everything is fine 

                    }
                    else
                    {

                        ViewBag.Error = "Client not found"; // if client is not found, it assign string to view bag 


                    }


                }
                else
                {

                    ViewBag.Error = responce.StatusCode.ToString(); // if bad request, it stores statuscode as string to view bag 

                }

                return View();
            }
            catch (Exception ex)
            {

                ViewBag.Error = ex.Message; // if exceotion thrown, it assign error message to view bag 
                return View();
            }




        }



    }
}
