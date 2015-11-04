using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NextLAP.IP1.PlanningWebAPI.Models.Base;

namespace NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan.Post
{
    /// <summary>
    /// Id points to EquipmentConfiguration.Id
    /// </summary>
    public class ConfigureEquipmentConfigurationModel : BaseModel
    {
        [JsonProperty("ip")]
        public string IpAddress { get; set; }
    }
}