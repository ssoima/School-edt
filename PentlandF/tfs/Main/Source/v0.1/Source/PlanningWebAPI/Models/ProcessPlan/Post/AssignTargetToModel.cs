using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NextLAP.IP1.PlanningWebAPI.Models.Base;

namespace NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan.Post
{
    public class AssignTargetToModel : BaseModel
    {
        [JsonProperty("target_id")]
        public long? TargetId { get; set; }
    }
}