using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Base;

namespace NextLAP.IP1.Models.Equipment
{
    public class EquipmentDriver : ExtendedEntity
    {
        public long? EquipmentTypeId { get; set; }
        public virtual EquipmentType EquipmentType { get; set; }
        public string ClrType { get; set; }
    }
}
