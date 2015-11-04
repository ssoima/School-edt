using NextLAP.IP1.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NextLAP.IP1.Models.Planning
{
    public class ManufacturingModel : ExtendedTimedEntity
    {
        // f.e. Limousince oder PRNR
        public string Family { get; set; }
        // f.e. A4 oder aber auch PRNR ( == 1X1)
        public string Code { get; set; }
    }
}