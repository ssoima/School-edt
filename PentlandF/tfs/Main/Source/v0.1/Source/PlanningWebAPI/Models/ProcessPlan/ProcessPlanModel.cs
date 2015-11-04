using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NextLAP.IP1.Models.Structure;
using NextLAP.IP1.PlanningWebAPI.Models.Base;

namespace NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan
{
    public class ProcessPlanModel : NamedBaseModel
    {
        public int Version { get; set; }
        public long FactoryId { get; set; }
    }
}