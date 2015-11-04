using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NextLAP.IP1.Common;
using NextLAP.IP1.PlanningWebAPI.Models.Base;

namespace NextLAP.IP1.PlanningWebAPI.Models.PlantLayout.Post
{
    public class CreateStationWithWorkerModel : NamedBaseModel, ILinkedEntity
    {
        [JsonProperty("section")]
        public long SectionId { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("worker")]
        public int Worker { get; set; }
        [JsonProperty("lft")]
        public long? PredecessorId { get; set; }
    }
}