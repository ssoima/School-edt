using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace NextLAP.IP1.PlanningWebAPI.Models
{
    public class KeyValue
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        [JsonProperty("val")]
        public string Value { get; set; }
    }
}