using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NextLAP.IP1.Common.Web.Models;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    public class EquipmentDescriptor : BaseModel
    {
        [JsonProperty("type")]
        public string EquipmentType { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("conf")]
        public string Configuration { get; set; }
        [JsonProperty("ip")]
        public string IpAddress { get; set; }
        [JsonProperty("drv")]
        public string Driver { get; set; }
    }
}
