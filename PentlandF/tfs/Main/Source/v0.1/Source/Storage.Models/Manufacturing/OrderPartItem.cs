using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Base;
using NextLAP.IP1.Models.Planning;

namespace NextLAP.IP1.Models.Manufacturing
{
    public class OrderPartItem : BaseEntity
    {
        public long? OrderId { get; set; }
        public virtual Order Order { get; set; }
        public long? PartId { get; set; }
        public Part Part { get; set; }
    }
}
