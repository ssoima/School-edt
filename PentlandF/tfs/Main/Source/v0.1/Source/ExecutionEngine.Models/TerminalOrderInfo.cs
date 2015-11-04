using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    public class TerminalOrderInfo
    {
        [JsonProperty("sequence")]
        public OrderSequence OrderSequence { get; set; }
        [JsonProperty("current")]
        public string OrderNumber { get; set; }
        [JsonProperty("customer")]
        public string Customer { get; set; }
        [JsonProperty("model")]
        public string Model { get; set; }
        [JsonProperty("variant")]
        public string Variant { get; set; }
        [JsonProperty("stationId")]
        public long StationId { get; set; }
        [JsonProperty("workstationId")]
        public long WorkstationId { get; set; }
        [JsonProperty("pos")]
        public double CurrentPosition { get; set; }
        [JsonProperty("tasks")]
        public ICollection<TerminalTaskSequenceInfo> TaskSequenceInfos { get; set; }
    }

    public class OrderSequence
    {
        [JsonProperty("prev")]
        public string Previous { get; set; }
        [JsonProperty("current")]
        public string Current { get; set; }
        [JsonProperty("next")]
        public string Next { get; set; }
    }
}
