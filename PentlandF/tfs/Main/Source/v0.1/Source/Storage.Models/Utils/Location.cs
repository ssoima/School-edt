using NextLAP.IP1.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NextLAP.IP1.Models.Utils
{
    public class Location : BaseEntity
    {
        public string Country { get; set; } //later would be changed to it's own Entity
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string HouseNumber { get; set; }
    }
}