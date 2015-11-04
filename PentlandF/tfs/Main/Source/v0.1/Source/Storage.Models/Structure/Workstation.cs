using NextLAP.IP1.Models.Base;
using NextLAP.IP1.Models.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NextLAP.IP1.Models.Planning;

namespace NextLAP.IP1.Models.Structure
{
    public class Workstation : ExtendedTimedEntity
    {
        public string Side { get; set; }
        public string Type { get; set; }
        public int Position { get; set; }
        public long StationId { get; set; }
        public virtual Station Station { get; set; }
        public long? WorkStationTerminalId { get; set; }
        public virtual  WorkstationTerminal WorkStationTerminal { get; set; }
        public virtual ICollection<EquipmentConfiguration> EquipmentConfigurations { get; set; }
        public virtual ICollection<StationTaskAssignment> TaskAssignments { get; set; }
    }
}