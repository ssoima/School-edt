using NextLAP.IP1.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NextLAP.IP1.Models.Equipment;
using NextLAP.IP1.Models.Structure;
namespace NextLAP.IP1.Models.Planning
{
    public class Task : ExtendedTimedEntity
    {
        public Action Action { get; set; }
        public bool NeedsEquipment { get; set; }
        public bool ShowInTerminal { get; set; }
    }
    public enum Action
    {
        Go,
        Pick,
        Scan,
        Screw,
        Mount,
        Clip,
        Review,
        DoNothing
    }
}