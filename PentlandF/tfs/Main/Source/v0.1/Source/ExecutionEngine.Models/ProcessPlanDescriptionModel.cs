using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    public class ProcessPlanDescriptionModel
    {
        [JsonProperty("stations")]
        public ICollection<StationTasksDescriptor> StationWorkload { get; set; }
    }
}
