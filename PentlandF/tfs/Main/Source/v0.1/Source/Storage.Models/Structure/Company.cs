using NextLAP.IP1.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NextLAP.IP1.Models.Structure
{
    public class Company : ExtendedEntity
    {
        public ICollection<Factory> Factories { get; set; }
    }
}