using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace NextLAP.IP1.ExecutionEngineWebAPI.Models
{
    public class ManufacturingOrderModel
    {
        [JsonProperty("no")]
        public string OrderNumber { get; set; }
        [JsonProperty("customer")]
        public string Customer { get; set; }
        [JsonProperty("orderDate")]
        public DateTime OrderDate { get; set; }
        [JsonProperty("model")]
        public string Model { get; set; }
        [JsonProperty("variant")]
        public string Variant { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}