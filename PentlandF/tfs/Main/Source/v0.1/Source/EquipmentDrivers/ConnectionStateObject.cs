using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NextLAP.IP1.EquipmentDrivers
{
    internal class ConnectionStateObject
    {
        public const int DefaultBufferSize = 512;

        public ConnectionStateObject() : this(DefaultBufferSize)
        {
        }

        public ConnectionStateObject(int bufferSize)
        {
            Result = new StringBuilder();
            Buffer = new byte[bufferSize];
        }

        public Socket Client { get; set; }
        public byte[] Buffer { get; set; }
        public StringBuilder Result { get; private set; }
    }
}
