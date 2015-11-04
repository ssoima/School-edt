using NextLAP.IP1.Common;
using NextLAP.IP1.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NextLAP.IP1.Models.Structure
{
    public class Section : ExtendedTimedEntity, ILinkedEntity<Section>
    {
        public long? PredecessorId { get; set; }
        public virtual Section Predecessor { get; set; }
        public long? AssemblyLineId { get; set; }
        public virtual AssemblyLine AssemblyLine { get; set; }

    }
}