using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace NextLAP.IP1.PlanningWebAPI.Models.Base
{
    public class BaseModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }
    }
}