using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions.Models
{
    public class Product //Ira, 12/10/23 
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductMaterial { get; set; }
        public string ProductColour { get; set; }
        public int ProductQty { get; set; }
        public string ProductImg { get; set; }
        public double ProductPrice { get; set; }
        public int CategoryID { get; set; }
        public int BrandID { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
    }
}
