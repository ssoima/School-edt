using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NextLAP.IP1.Common;
using NextLAP.IP1.Common.Web.Models;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    public class StationModel : NamedBaseModel, ILinkedEntity
    {
        [JsonProperty("section")]
        public long? SectionId { get; set; }
        [JsonProperty("lft")]
        public long? PredecessorId { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("length")]
        public int Length { get; set; }
    }
}