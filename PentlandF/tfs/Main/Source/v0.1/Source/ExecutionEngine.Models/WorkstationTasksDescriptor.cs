using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    public class WorkstationTasksDescriptor
    {
        [JsonProperty("id")]
        public long WorkstationId { get; set; }
        [JsonProperty("name")]
        public string WorkstationName { get; set; }
        [JsonProperty("tasks")]
        public ICollection<TaskDescriptor> Tasks { get; set; }
        [JsonProperty("equipments")]
        public ICollection<EquipmentDescriptor> Equipments { get; set; }
    }
}
