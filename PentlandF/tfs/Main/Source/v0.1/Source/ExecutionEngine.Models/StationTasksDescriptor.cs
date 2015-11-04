using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    public class StationTasksDescriptor
    {
        [JsonProperty("id")]
        public long StationId { get; set; }
        [JsonProperty("name")]
        public string StationName { get; set; }
        [JsonProperty("workstations")]
        public ICollection<WorkstationTasksDescriptor> WorkstationWorkloads { get; set; }
    }
}
