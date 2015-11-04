using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NextLAP.IP1.Common.Web.Models;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    public class TaskDescriptor : BaseModel
    {
        [JsonProperty("preId")]
        public long? PredecessorId { get; set; }
        [JsonProperty("tId")]
        public long TaskId { get; set; }
        [JsonProperty("t")]
        public string Task { get; set; }
        [JsonProperty("pId")]
        public long PartId { get; set; }
        [JsonProperty("p")]
        public string Part { get; set; }
        [JsonProperty("p_no")]
        public string PartNumber { get; set; }
        [JsonProperty("img")]
        public string ImageUrl { get; set; }
        [JsonProperty("show")]
        public bool ShowInTerminal { get; set; }
        [JsonProperty("eq_req")]
        public bool IsEquipmentRequired { get; set; }
        [JsonProperty("eq_id")]
        public long? EquipmentId { get; set; }
        [JsonProperty("drv_conf")]
        public ICollection<KeyValue> DriverConfiguration { get; set; }
    }
}
