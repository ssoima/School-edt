using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NextLAP.IP1.Common;
using NextLAP.IP1.PlanningWebAPI.Models.Base;

namespace NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan
{
    public class StationTaskModel : BaseModel, ILinkedEntity
    {
        public long? PredecessorId { get; set; }
        public PartModel Part { get; set; }
        public TaskModel Task { get; set; }
        public TimeSpan? ExpectedTime { get; set; }
        public long? AssignedStationId { get; set; }
        public long? AssignedWorkstationId { get; set; }
    }
}