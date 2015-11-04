using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    public class ConveyanceProgressModel
    {
        [JsonProperty("pos")]
        public double Position { get; set; }
        [JsonProperty("len")]
        public double TotalLength { get; set; }
        [JsonProperty("interval")]
        public double UpdateInterval { get; set; }
        [JsonProperty("speed")]
        public double Speed { get; set; }
        [JsonProperty("max")]
        public bool MaxReached { get; set; }
    }
}
