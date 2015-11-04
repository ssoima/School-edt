using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NextLAP.IP1.Common.Web.Models;

namespace NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan.Post
{
    public class RemoveDriverConfigurationModel : BaseModel
    {
        [JsonProperty("key")]
        public string Key { get; set; }
    }
}