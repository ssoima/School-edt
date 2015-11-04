using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NextLAP.IP1.PlanningWebAPI.Models.Base;

namespace NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan
{
    public class EquipmentDriverConfigurationValueModel : BaseModel
    {
        [JsonProperty("key")]
        public string Name { get; set; }
        [JsonProperty("val")]
        public string Value { get; set; }
    }
}