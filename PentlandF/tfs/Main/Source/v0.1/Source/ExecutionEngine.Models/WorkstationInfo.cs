using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    public class WorkstationInfo
    {
        [JsonProperty("station")]
        public string Station { get; set; }
        [JsonProperty("stationId")]
        public long StationId { get; set; }
        [JsonProperty("workstation")]
        public string Workstation { get; set; }
        [JsonProperty("workstationId")]
        public long WorkstationId { get; set; }
        [JsonProperty("len")]
        public int Length { get; set; }
        [JsonProperty("offset")]
        public int Offset { get; set; }
        [JsonProperty("speed")]
        public double Speed { get; set; }
    }
}
