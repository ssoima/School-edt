using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NextLAP.IP1.PlanningWebAPI.Models.Base;

namespace NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan
{
    public class EquipmentDriverModel : NamedBaseModel
    {
        [JsonProperty("typeId")]
        public long? EquipmentTypeId { get; set; }
        [JsonProperty("type")]
        public string EquipmentType { get; set; }
        [JsonProperty("drv")]
        public string ClrType { get; set; }
    }
}