using NextLAP.IP1.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NextLAP.IP1.Models.Utils;

namespace NextLAP.IP1.Models.Structure
{
    public class Factory : ExtendedTimedEntity
    {
        public long? LocationId { get; set; }
        public virtual Location Location { get; set; }
        public long? CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public ICollection<AssemblyLine> AssemblyLines { get; set; }
    }
}