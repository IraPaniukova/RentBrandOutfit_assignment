namespace RentBrandOutfit.Models
{
    public class ProductDetailed  //Ira 23/10/23
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductMaterial { get; set; }
        public string ProductColour { get; set; }
        public string ProductQty { get; set; }
        public string ProductImg { get; set; }
        public double ProductPrice { get; set; }
        public int CategoryID { get; set; }
        public int BrandID { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
    }
}
