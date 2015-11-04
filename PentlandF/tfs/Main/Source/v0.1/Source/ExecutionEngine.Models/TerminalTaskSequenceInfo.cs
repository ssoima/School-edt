using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    public class TerminalTaskSequenceInfo
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("wsId")]
        public long WorkstationId { get; set; }
        [JsonProperty("order")]
        public int Order { get; set; }
        [JsonProperty("t")]
        public string Task { get; set; }
        [JsonProperty("p")]
        public string Part { get; set; }
        [JsonProperty("img")]
        public string ImageUrl { get; set; }
        [JsonProperty("eq")]
        public string Equipment { get; set; }
        [JsonProperty("eq_req")]
        public bool IsEquipmentRequired { get; set; }
        [JsonProperty("progress")]
        public bool InProgress { get; set; }
        [JsonProperty("suc")]
        public bool? Success { get; set; }
        [JsonProperty("c")]
        public string Comment { get; set; }
    }
}
