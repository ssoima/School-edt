using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NextLAP.IP1.Common;
using NextLAP.IP1.PlanningWebAPI.Models.Base;

namespace NextLAP.IP1.PlanningWebAPI.Models.PlantLayout
{
    public class SectionModel : NamedBaseModel, ILinkedEntity
    {
        [JsonProperty("lft")]
        public long? PredecessorId { get; set; }
    }
}