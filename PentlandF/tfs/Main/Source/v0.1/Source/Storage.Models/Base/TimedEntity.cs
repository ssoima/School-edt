using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NextLAP.IP1.Models.Base
{
    public class TimedEntity : BaseEntity
    {
        /// <summary>
        /// Describes a general availability of a particular entity. 
        /// </summary>
        public DateTime? ValidFrom { get; set; }
        /// <summary>
        /// Describes an incremental version of a particular entity.
        /// </summary>
        public int Version { get; set; }
    }
}