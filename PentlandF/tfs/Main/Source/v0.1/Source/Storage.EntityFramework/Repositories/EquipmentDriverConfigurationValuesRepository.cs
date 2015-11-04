using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Equipment;

namespace NextLAP.IP1.Storage.EntityFramework.Repositories
{
    public class EquipmentDriverConfigurationValuesRepository : Ip1BaseRepository<EquipmentDriverConfigurationValue>
    {
        public EquipmentDriverConfigurationValuesRepository(Ip1Context context) : base(context)
        {
        }
    }
}
