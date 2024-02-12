namespace RentBrandOutfit.Models
{
    public class ClientLocation //Ira,16/10/23 I need this model to cinsume 2 fucntions in one view
    {
       
        public List<ClientDetailed>? Clients { get; set; }
        public List<Location>? Locations { get; set; }
        
    }
}
