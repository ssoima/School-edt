using NextLAP.IP1.Models.Base;
using NextLAP.IP1.Models.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NextLAP.IP1.Models.Structure
{
    public class Worker : BaseEntity
    {
        public string WorkerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}