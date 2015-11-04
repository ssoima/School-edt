using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace NextLAP.IP1.Common.Web.Models
{
    public class BaseModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }
    }
}