using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Base;

namespace NextLAP.IP1.Models.Equipment
{
    public class EquipmentDriverConfigurationValue : BaseEntity
    {
        public long? EquipmentDriverConfigurationId { get; set; }
        public virtual EquipmentDriverConfiguration EquipmentDriverConfiguration { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
