using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Base;

namespace NextLAP.IP1.Models.Manufacturing
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? PlannedDeliveryDate { get; set; }
        public string CustomerName { get; set; }
        public string Model { get; set; }
        public string Variant { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime? StatusChanged { get; set; }
        public virtual ICollection<OrderPartItem> Parts { get; set; }
    }

    public enum OrderStatus
    {
        New,
        Queued,
        InProgress,
        Complete
    }
}
