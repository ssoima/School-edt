using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using NextLAP.IP1.PlanningWebAPI.Models.Base;

namespace NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan
{
    /// <summary>
    /// The assignable part list model is intended to deliver a list of parts and their assigned tasks, so users can assign those tasks to stations.
    /// </summary>
    public class AssignablePartListModel : NamedBaseModel
    {
        [JsonProperty("number")]
        public string PartNumber { get; set; }
        [JsonProperty("status")]
        public string IsCompletelyAssigned
        {
            get
            {
                var assigned = AssignableTasks.Where(x => x.IsAssigned).ToList();
                if(assigned.Count == 0) return AssignmentStatus.NothingAssigned;
                
                var all = AssignableTasks.ToList();
                return assigned.Count != all.Count
                    ? AssignmentStatus.PartiallyAssigned
                    : AssignmentStatus.CompletelyAssigned;
            }
        }
        [JsonProperty("tasks")]
        public IEnumerable<AssignableTaskModel> AssignableTasks { get; set; }
    }

    public static class AssignmentStatus
    {
        public const string NothingAssigned = "nothing";

        public const string PartiallyAssigned = "partial";

        public const string CompletelyAssigned = "complete";
    }
}