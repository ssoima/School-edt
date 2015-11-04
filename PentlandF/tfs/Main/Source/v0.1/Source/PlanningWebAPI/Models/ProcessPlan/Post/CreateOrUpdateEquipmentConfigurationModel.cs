using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NextLAP.IP1.PlanningWebAPI.Models.Base;

namespace NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan.Post
{
    public class CreateOrUpdateEquipmentConfigurationModel : NamedBaseModel
    {
        [JsonProperty("equipmentTypeId")]
        public long EquipmentTypeId { get; set; }
        [JsonProperty("workstationId")]
        public long WorkstationId { get; set; }
        [JsonProperty("driverId")]
        public long? DriverId { get; set; }
        [JsonProperty("address")]
        public string IpAddress { get; set; }
        [JsonProperty("config")]
        public string Configuration { get; set; }
    }
}