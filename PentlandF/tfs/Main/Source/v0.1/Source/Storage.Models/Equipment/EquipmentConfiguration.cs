using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Base;
using NextLAP.IP1.Models.Planning;
using NextLAP.IP1.Models.Structure;

namespace NextLAP.IP1.Models.Equipment
{
    public class EquipmentConfiguration : ExtendedEntity
    {
        public long? EquipmentTypeId { get; set; }
        public virtual EquipmentType EquipmentType { get; set; }
        public long? UsedDriverId { get; set; }
        public virtual EquipmentDriver UsedDriver { get; set; }
        public long? WorkStationId { get; set; }
        public virtual Workstation WorkStation { get; set; }
        public string IpAddress { get; set; }
        public string Configuration { get; set; }
        public virtual ICollection<StationTaskAssignment> StationTaskAssignments { get; set; }
    }
}
