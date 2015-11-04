﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace NextLAP.IP1.PlanningWebAPI.Models.Base
{
    public class CountModel : BaseModel
    {
        [JsonProperty("cnt")]
        public int Count { get; set; }
    }
}