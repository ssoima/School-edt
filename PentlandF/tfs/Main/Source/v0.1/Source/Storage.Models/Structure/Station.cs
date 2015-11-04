using NextLAP.IP1.Common;
using NextLAP.IP1.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NextLAP.IP1.Models.Planning;

namespace NextLAP.IP1.Models.Structure
{
    public class Station : ExtendedTimedEntity, ILinkedEntity<Station>
    {
        public TimeSpan? OffsetTime { get; set; }
        public int? OffsetLength { get; set; }
        public long? PredecessorId { get; set; }
        public virtual Station Predecessor { get; set; }
        public long? StationTypeId { get; set; }
        public virtual StationType StationType { get; set; }
        public long? SectionId { get; set; }
        public virtual Section Section { get; set; }
        public virtual ICollection<Workstation> WorkStations { get; set; }
        public virtual ICollection<StationTaskAssignment> TaskAssignments { get; set; } 
    }
}