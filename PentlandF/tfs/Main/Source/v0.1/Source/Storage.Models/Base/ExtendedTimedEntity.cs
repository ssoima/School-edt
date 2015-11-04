using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NextLAP.IP1.Models.Base
{
    public class ExtendedTimedEntity : TimedEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}