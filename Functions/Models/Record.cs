using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions.Models
{
    internal class Record
    {
       
        public DateTime RentStartDate { get; set; }
        public DateTime RentEndDate { get; set; }
        public int RentQuantity { get; set; }
        public decimal RentTotalPrice { get; set; }
        public int ClientID { get; set; }
        public int ProductID { get; set; }
        public int EmployeeID { get; set; }



    }
}
