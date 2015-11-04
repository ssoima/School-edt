using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Base;
using NextLAP.IP1.Models.Structure;

namespace NextLAP.IP1.Models.Planning
{
    public class ProcessPlan : ExtendedTimedEntity
    {
        public virtual ICollection<StationTaskAssignment> StationTaskAssignments { get; set; }
        public int CurrentVersion { get; set; }
        public long? FactoryId { get; set; }
        public virtual Factory Factory { get; set; }
    }
}
