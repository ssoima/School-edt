using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace NextLAP.IP1.ExecutionEngineWebAPI.Models
{
    public class EnqueueNewOrderModel
    {
        [JsonProperty("order")]
        public string OrderNumber { get; set; }
    }
}