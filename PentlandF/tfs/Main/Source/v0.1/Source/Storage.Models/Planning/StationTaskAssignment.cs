using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Common;
using NextLAP.IP1.Models.Base;
using NextLAP.IP1.Models.Equipment;
using NextLAP.IP1.Models.Structure;

namespace NextLAP.IP1.Models.Planning
{
    public class StationTaskAssignment : BaseTaskAssignment, ILinkedEntity<StationTaskAssignment>
    {
        public long? AssignedProcessPlanId { get; set; }
        public virtual ProcessPlan AssignedProcessPlan { get; set; }
        public long? PredecessorId { get; set; }
        public virtual StationTaskAssignment Predecessor { get; set; }
        public long? EquipmentConfigurationId { get; set; }
        public virtual EquipmentConfiguration EquipmentConfiguration { get; set; }
        public long? EquipmentDriverConfigurationId { get; set; }
        public virtual EquipmentDriverConfiguration EquipmentDriverConfiguration { get; set; }
        public long? WorkstationId { get; set; }
        public virtual Workstation Workstation { get; set; }
        public long? StationId { get; set; }
        public virtual Station Station { get; set; }
        public long? BasedOnTaskAssignmentId { get; set; }
        public virtual TaskAssignment BasedOnTaskAssignment { get; set; }
        public bool ShowInTerminal { get; set; }
    }
}
