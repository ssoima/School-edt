using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Manufacturing;

namespace NextLAP.IP1.Storage.EntityFramework.Repositories
{
    public class OrderRepository : Ip1BaseRepository<Order>
    {
        public OrderRepository(Ip1Context context) : base(context)
        {
        }
    }
}
