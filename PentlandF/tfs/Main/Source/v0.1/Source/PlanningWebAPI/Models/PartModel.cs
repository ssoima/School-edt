using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NextLAP.IP1.PlanningWebAPI.Models.Base;

namespace NextLAP.IP1.PlanningWebAPI.Models
{
    public class PartModel : NamedBaseModel
    {
        [JsonProperty("number")]
        public string Number { get; set; }
        [JsonProperty("longName")]
        public string LongName { get; set; }
        [JsonProperty("partType")]
        public string PartType { get; set; }
        [JsonProperty("partGroup")]
        public string PartGroup { get; set; }
    }
}