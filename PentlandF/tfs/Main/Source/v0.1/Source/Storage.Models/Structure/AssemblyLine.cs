using NextLAP.IP1.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NextLAP.IP1.Models.Structure
{
    public class AssemblyLine : ExtendedTimedEntity
    {
        public long? FactoryId { get; set; }
        public virtual Factory Factory { get; set; }
        public string Type { get; set; } // Main, Preassembly, Painting, Body construction
        public TimeSpan? Time { get; set; }
        public ICollection<Section> Sections { get; set; }
    }
}