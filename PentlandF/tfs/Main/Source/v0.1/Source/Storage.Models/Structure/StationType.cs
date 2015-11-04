using NextLAP.IP1.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NextLAP.IP1.Models.Structure
{
    public class StationType : BaseEntity
    {
        // length in cm (int32 should be big enough)
        public int Length { get; set; }
        public string Name { get; set; }
    }
}