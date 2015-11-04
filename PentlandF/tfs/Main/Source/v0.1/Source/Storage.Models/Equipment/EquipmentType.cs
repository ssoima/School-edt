using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Base;

namespace NextLAP.IP1.Models.Equipment
{
    public class EquipmentType : ExtendedEntity
    {
        public virtual ICollection<EquipmentDriver> AvailableDrivers { get; set; }
    }
}
