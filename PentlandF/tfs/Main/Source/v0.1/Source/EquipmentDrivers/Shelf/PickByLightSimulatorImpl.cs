using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace NextLAP.IP1.EquipmentDrivers.Shelf
{
    public sealed class PickByLightSimulatorImpl : BaseTcpEquipment
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public PickByLightSimulatorImpl() : base(true)
        {
        }

        protected override void SendCommand(Socket client, Dictionary<string, string> commands)
        {
            var command = string.Empty;
            commands.TryGetValue("cmd", out command);
            var bytes = Encoding.ASCII.GetBytes(command + "\r\n");
            client.BeginSend(bytes, 0, bytes.Length, 0, SendCallback, client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            client.EndSend(ar);
        }
    }
}
