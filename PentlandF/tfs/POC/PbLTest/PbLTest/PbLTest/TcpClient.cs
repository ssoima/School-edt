using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PbLTest
{
    public class TcpClient
    {
        private Socket tcp;
        private string receiveBuffer;

        public TcpClient() { }

        public void Connect(string host, int port)
        {

            if (tcp != null)
                Disconnect();

            tcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcp.Connect(host, port);

            // Create the state object.
            TcpIpReceiveCallbackStateObject state = new TcpIpReceiveCallbackStateObject();
            state.Client = tcp;

            // Begin receiving the data from the remote device.
            tcp.BeginReceive(state.Buffer, 0, TcpIpReceiveCallbackStateObject.BufferSize, 0, new AsyncCallback(TcpIpReceiveCallback), state);
        }

        public void Disconnect()
        {
            if (tcp != null && tcp.Connected == true)
            {
                tcp.Shutdown(SocketShutdown.Both);
                tcp.Close();
            }
        }

        public bool Connected
        {
            get { return tcp != null ? tcp.Connected : false; }
        }

        public void Send(string data)
        {
            if (tcp != null && tcp.Connected == true)
            {
                tcp.Send(Encoding.ASCII.GetBytes(data));
            }
        }

        private void TcpIpReceiveCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the client socket from the asynchronous state object.             
            TcpIpReceiveCallbackStateObject state = (TcpIpReceiveCallbackStateObject)ar.AsyncState;
            Socket client = state.Client;
            try
            {
                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    receiveBuffer = Encoding.ASCII.GetString(state.Buffer, 0, bytesRead);
                    if (DataReceived != null)
                    {
                        DataReceived(this, new DataReceviedEventArgs() { ReceivedString = receiveBuffer, ReceivedData = state.Buffer });
                    }

                    //Continue receiving data.
                    client.BeginReceive(state.Buffer, 0, TcpIpReceiveCallbackStateObject.BufferSize, 0, new AsyncCallback(TcpIpReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived
                }
            }
            catch (ObjectDisposedException)
            {
                //Do not throw Exception, Socket might have been closed already
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public event EventHandler<DataReceviedEventArgs> DataReceived;

        private class TcpIpReceiveCallbackStateObject
        {
            /// <summary>
            /// Client socket.
            /// </summary>
            public Socket Client;
            /// <summary>
            /// Size of receive buffer.
            /// </summary>
            public const int BufferSize = 1024;
            // Receive buffer.
            public byte[] Buffer = new byte[BufferSize];
        }
    }

    public class DataReceviedEventArgs : EventArgs
    {
        public string ReceivedString { get; set; }
        public byte[] ReceivedData { get; set; }
    }
}
