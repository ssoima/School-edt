using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NextLAP.IP1.PlanningWebAPI.Models.Base;

namespace NextLAP.IP1.PlanningWebAPI.Models.PlantLayout
{
    public class StationTypeModel : BaseModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("len")]
        public int Length { get; set; }
    }
}