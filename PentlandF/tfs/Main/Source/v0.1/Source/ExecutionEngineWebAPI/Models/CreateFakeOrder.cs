using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace NextLAP.IP1.ExecutionEngineWebAPI.Models
{
    public class CreateFakeOrder
    {
        [JsonProperty("customer")]
        public string CustomerName { get; set; }
        [JsonProperty("variant")]
        public string Variant { get; set; }
    }
}