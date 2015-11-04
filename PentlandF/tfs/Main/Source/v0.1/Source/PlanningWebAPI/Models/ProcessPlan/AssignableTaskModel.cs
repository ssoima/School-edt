using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NextLAP.IP1.PlanningWebAPI.Models.Base;

namespace NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan
{
    /// <summary>
    /// An assignable task model is a representation of the task assignment entity
    /// </summary>
    public class AssignableTaskModel : NamedBaseModel
    {
        [JsonProperty("isAssigned")]
        public bool IsAssigned
        {
            get { return StationdId != null; }
        }
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
        [JsonProperty("equipmentRequired")]
        public bool NeedsEquipment { get; set; }
        [JsonProperty("equipmentId")]
        public long? AssignedEquipmentId { get; set; }
        [JsonProperty("equipment")]
        public string AssignedEquipment { get; set; }
        [JsonProperty("currentAssignmentId")]
        public long? CurrentStationTaskAssignmentId { get; set; }
    }
}