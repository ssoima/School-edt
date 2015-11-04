using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NextLAP.IP1.Common;
using NextLAP.IP1.PlanningWebAPI.Models.Base;

namespace NextLAP.IP1.PlanningWebAPI.Models
{
    public class MoveLinkedEntityModel : BaseModel, ILinkedEntity
    {
        [JsonProperty("lft")]
        public long? PredecessorId { get; set; }
    }
}