using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Base;

namespace NextLAP.IP1.Models.Equipment
{
    public class WorkstationTerminal : BaseEntity
    {
        /// <summary>
        /// This GUID will be transfered to the server when the Workstation Computer starts a communication channel to the Server via SignalR.
        /// </summary>
        public Guid ConnectionIdentifier { get; set; }
    }
}
