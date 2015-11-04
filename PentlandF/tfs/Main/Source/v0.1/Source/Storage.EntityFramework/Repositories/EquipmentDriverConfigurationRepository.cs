using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Equipment;

namespace NextLAP.IP1.Storage.EntityFramework.Repositories
{
    public class EquipmentDriverConfigurationRepository : Ip1BaseRepository<EquipmentDriverConfiguration>
    {
        public EquipmentDriverConfigurationRepository(Ip1Context context) : base(context)
        {
        }

        public void DeleteEquipmentDriverConfiguration(long id)
        {
            var entity = Entities.FirstOrDefault(x => x.Id == id);
            if(entity == null) return;
            Delete(entity);
            SaveChanges();
        }
    }
}
