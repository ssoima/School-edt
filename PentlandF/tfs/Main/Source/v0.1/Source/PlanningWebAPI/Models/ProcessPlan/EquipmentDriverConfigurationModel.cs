using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NextLAP.IP1.PlanningWebAPI.Models.Base;

namespace NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan
{
    /// <summary>
    /// Id points to EquipmentDriverConfiguration.Id
    /// </summary>
    public class EquipmentDriverConfigurationModel : BaseModel
    {
        //[JsonProperty("equipmentId")]
        //public long EquipmentConfigurationId { get; set; }
        [JsonProperty("values")]
        public ICollection<EquipmentDriverConfigurationValueModel> Values { get; set; }
    }
}