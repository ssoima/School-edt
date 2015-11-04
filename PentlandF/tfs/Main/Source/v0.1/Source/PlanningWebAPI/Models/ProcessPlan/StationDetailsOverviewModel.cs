using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NextLAP.IP1.PlanningWebAPI.Models.Base;
using NextLAP.IP1.PlanningWebAPI.Models.PlantLayout;

namespace NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan
{
    public class StationDetailsOverviewModel : NamedBaseModel
    {
        [JsonProperty("workers")]
        public IEnumerable<WorkstationModel> Workstations { get; set; }
        [JsonProperty("assignedTasks")]
        public IEnumerable<StationDetailsTaskModel> AssignedTasks { get; set; } 
    }
}