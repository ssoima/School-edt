using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Equipment;

namespace NextLAP.IP1.Storage.EntityFramework.Repositories
{
    public class EquipmentDriverRepository : Ip1BaseRepository<EquipmentDriver>
    {
        public EquipmentDriverRepository(Ip1Context context) : base(context)
        {
        }
    }
}
