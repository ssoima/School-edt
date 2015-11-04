using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NextLAP.IP1.PlanningWebAPI.Models.Base;

namespace NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan
{
    public class EquipmentConfigurationModel : NamedBaseModel
    {
        [JsonProperty("address")]
        public string IpAddress { get; set; }
        [JsonProperty("config")]
        public string Configuration { get; set; }
        [JsonProperty("typeId")]
        public long? EquipmentTypeId { get; set; }
        [JsonProperty("type")]
        public string EquipmentType { get; set; }
        [JsonProperty("driverId")]
        public long? AssignedDriverId { get; set; }
        [JsonProperty("driver")]
        public string AssignedDriver { get; set; }
        [JsonProperty("workstationId")]
        public long? WorkstationId { get; set; }
        [JsonProperty("workstation")]
        public string Workstation { get; set; }
    }
}