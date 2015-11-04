using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NextLAP.IP1.Common.Web.Models;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    public class WorkstationModel : NamedBaseModel
    {
        [JsonProperty("side")]
        public string Side { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("position")]
        public int Position { get; set; }
        [JsonProperty("stationId")]
        public long? StationId { get; set; }
        [JsonProperty("station")]
        public string StationName { get; set; }
    }
}