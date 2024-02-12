namespace RentBrandOutfit.Models
{
    public class RentResponce
    {


        public DateTime RentStartDate { get; set; }
        public DateTime RentEndDate { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductImg { get; set; }
        public string ClientName { get; set; }
        public int ClientID { get; set; }
        public decimal total { get; set; }
        public int days { get; set; }
        public int QTY { get; set; }
        public decimal price { get; set; }
    }
}