using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NextLAP.IP1.EquipmentDrivers
{
    public abstract class BaseTcpEquipment : IEquipment
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        protected TcpClient _tcpConnection;
        private bool _connected;
        private string _connectedHost;
        private int _connectedPort;
        private bool _wasConnected;
        private bool _constantRead;

        public BaseTcpEquipment(bool constantRead)
        {
            _tcpConnection = new TcpClient();
            _constantRead = constantRead;
        }

        public bool Connected
        {
            get
            {
                return _tcpConnection.Connected;
            }
        }

        public event EventHandler<MessageReceivedEventArgs> Receive;

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                var state = (ConnectionStateObject)ar.AsyncState;
                var client = state.Client;
                var bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    var str = Encoding.ASCII.GetString(state.Buffer, 0, bytesRead);
                    Log.Debug("received: " + str);
                    state.Result.Append(str);
                }
                
                if(Receive != null)
                {
                    Receive(this, new MessageReceivedEventArgs(new DefaultResponse(this, state.Result.ToString(), true)));
                }
                state = new ConnectionStateObject();
                state.Client = _tcpConnection.Client;
                client.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnReceive, state);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
        }

        public void Connect(string address)
        {
            var hostInfo = address.Split(':');
            if (hostInfo.Length != 2) throw new InvalidOperationException("Wrong format: Please specify <ip>:<port>");
            var host = hostInfo[0];
            var portString = hostInfo[1];
            int port = 0;
            if (!Int32.TryParse(portString, out port)) throw new InvalidOperationException("Port must be a number");
            Connect(host, port);
        }

        public void Connect(string host, int port)
        {
            _tcpConnection.Connect(host, port);
            _wasConnected = true;
            _connectedHost = host;
            _connectedPort = port;
            if (_constantRead)
            {
                var state = new ConnectionStateObject();
                state.Client = _tcpConnection.Client;
                _tcpConnection.Client.BeginReceive(state.Buffer, 0, state.Buffer.Length, 0, OnReceive, state);
            }
        }

        public void Send(Dictionary<string, string> commands)
        {
            if (!Connected) Connect(_connectedHost, _connectedPort);
            SendCommand(_tcpConnection.Client, commands);
        }

        protected abstract void SendCommand(Socket client, Dictionary<string, string> commands);

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _tcpConnection.Close();
                }
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~BaseTcpEquipment() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class DefaultResponse : IEquipmentResponse
    {
        public DefaultResponse(IEquipment equipment, string message, bool success)
        {
            Source = equipment;
            Message = message;
            Success = success;
        }

        public string Message
        {
            get; private set;
        }

        public IEquipment Source
        {
            get; private set;
        }

        public bool Success
        {
            get; private set;
        }
    }
}
