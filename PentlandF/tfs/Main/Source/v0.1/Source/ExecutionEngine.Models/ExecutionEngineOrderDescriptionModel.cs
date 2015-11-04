using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    public class ExecutionEngineOrderDescriptionModel
    {
        [JsonProperty("no")]
        public string OrderNumber { get; set; }
        [JsonProperty("customer")]
        public string Customer { get; set; }
        [JsonProperty("model")]
        public string Model { get; set; }
        [JsonProperty("variant")]
        public string Variant { get; set; }
        [JsonProperty("stationtasks")]
        public ICollection<string> PartNumbers { get; set; }
    }
}
