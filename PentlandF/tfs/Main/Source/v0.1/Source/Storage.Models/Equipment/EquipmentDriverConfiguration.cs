using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Base;

namespace NextLAP.IP1.Models.Equipment
{
    public class EquipmentDriverConfiguration : BaseEntity
    {
        public virtual ICollection<EquipmentDriverConfigurationValue> Values { get; set; }
    }
}
