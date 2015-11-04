using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLAP.IP1.EquipmentDrivers
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(IEquipmentResponse reponse)
        {
            Response = reponse;
        }

        public IEquipmentResponse Response { get; private set; }
    }
}
