using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLAP.IP1.EquipmentDrivers
{
    public interface IEquipment : IDisposable
    {
        bool Connected { get; }
        void Connect(string address);
        void Send(Dictionary<string, string> commands);
        event EventHandler<MessageReceivedEventArgs> Receive;
    }
}
