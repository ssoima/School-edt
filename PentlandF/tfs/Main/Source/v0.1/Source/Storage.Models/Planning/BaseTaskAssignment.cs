using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Base;

namespace NextLAP.IP1.Models.Planning
{
    public class BaseTaskAssignment : ExtendedTimedEntity
    {
        public TimeSpan? Time { get; set; }
        public long? TaskId { get; set; }
        public virtual Task Task { get; set; }
        public long? PartId { get; set; }
        public virtual Part Part { get; set; }
        public long? TaskAssignmentImageId { get; set; }
        public virtual TaskAssignmentImage TaskAssignmentImage { get; set; }
    }
}
