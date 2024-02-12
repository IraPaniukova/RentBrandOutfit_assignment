using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions.Models
{
    public class Client //Ayako, Ira, 12/10/23
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientAddress { get; set; }
        public string ClientPhone { get; set; }
        public int LocationID { get; set; }
        
        public string LocationDescription { get; set; }
    }
}
