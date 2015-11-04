using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NextLAP.IP1.Common;
using NextLAP.IP1.PlanningWebAPI.Models.Base;

namespace NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan
{
    public class StationDetailsTaskModel : NamedBaseModel, ILinkedEntity
    {
        [JsonProperty("isAssigned")]
        public bool IsAssigned
        {
            get { return WorkstationId != null; }
        }
        [JsonProperty("part")]
        public string Part { get; set; }
        [JsonProperty("task")]
        public string Task { get; set; }
        [JsonProperty("stationId")]
        public long? StationdId { get; set; }
        [JsonProperty("station")]
        public string StationName { get; set; }
        [JsonProperty("workstationId")]
        public long? WorkstationId { get; set; }
        [JsonProperty("workstation")]
        public string WorkstationName { get; set; }
        [JsonProperty("equipmentId")]
        public long? EquipmentConfigurationId { get; set; }
        [JsonProperty("equipment")]
        public string EquipmentConfiguration { get; set; }
        [JsonProperty("equipment_config")]
        public string EquipmentConfigurationConfiguration { get; set; }
        [JsonProperty("equipmentDriverId")]
        public long? EquipmentDriverConfigurationId { get; set; }
        [JsonProperty("equipmentDriver_config")]
        public string EquipmentDriverConfiguration { get; set; }
        [JsonProperty("time")]
        public TimeSpan? Time { get; set; }
        [JsonProperty("lft")]
        public long? PredecessorId { get; set; }
        [JsonProperty("taskassignmentId")]
        public long? BasedOnTaskAssignmentId { get; set; }
        [JsonProperty("showOnWorkerscreen")]
        public bool ShowOnWorkerScreen { get; set; }
        [JsonProperty("hasImage")]
        public bool HasImage { get; set; }
    }
}