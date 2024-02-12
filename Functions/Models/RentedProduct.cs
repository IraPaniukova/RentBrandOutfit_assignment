using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions.Models
{
    internal class RentedProduct
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
