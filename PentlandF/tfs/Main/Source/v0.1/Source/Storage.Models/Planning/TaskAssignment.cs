using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Common;
using NextLAP.IP1.Models.Base;
using NextLAP.IP1.Models.Equipment;

namespace NextLAP.IP1.Models.Planning
{
    public class TaskAssignment : BaseTaskAssignment, ILinkedEntity<TaskAssignment>
    {
        public long? PredecessorId { get; set; }
        public virtual TaskAssignment Predecessor { get; set; }
        public long? ParentDependencyId { get; set; }
        public virtual TaskAssignment ParentDependency { get; set; }
        public virtual ICollection<TaskAssignment> Dependencies { get; set; }
        public virtual ICollection<StationTaskAssignment> StationTaskAssignments { get; set; }
        public long? NeededEquipmentTypeId { get; set; }
        public virtual EquipmentType NeededEquipmentType { get; set; }
        public long? ProposedDriverConfigurationId { get; set; }
        public virtual EquipmentDriverConfiguration ProposedDriverConfiguration { get; set; }
    }
}
