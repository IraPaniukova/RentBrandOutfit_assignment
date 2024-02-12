namespace RentBrandOutfit.Models
{
    public class RentedProductDetailed
    {
        public int RentID { get; set; }
        public DateTime RentStartDate { get; set; }
        public DateTime RentEndDate { get; set; }
        public int RentQuantity { get; set; }
        public double RentTotalPrice { get; set; }
        public string ProductImg { get; set; }
        public string ProductName { get; set; }
        public string ClientName { get; set; }
        public string EmployeeName { get; set; }

    }
}
