using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PickByLightSimulator.ConsoleClient
{
    public class StateObject
    {
        // Client socket.
        public Socket WorkSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 256;
        // Receive buffer.
        public byte[] Buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder String = new StringBuilder();
    }

    class Program
    {
        private static Thread _receiverThread = null;
        private static ManualResetEvent _sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent _connectDone =
        new ManualResetEvent(false);

        static void Main(string[] args)
        {
            try
            {
                using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {

                    client.BeginConnect("127.0.0.1", 13000, new AsyncCallback(ConnectCallback), client);
                    _connectDone.WaitOne();
                    SetupReceive(client);

                    Console.WriteLine("Connected..address a shelf slot to highlight it:");
                    Console.WriteLine("For example \"1:2\" will highlight slot in row 1 column 2..");
                    Console.WriteLine("-----------------------------------------------------------");
                    Console.WriteLine("To EXIT just hit ENTER");

                    string input;
                    while ((input = Console.ReadLine()) != "")
                    {
                        Send(client, input);
                        _sendDone.WaitOne();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
            if (_receiverThread != null)
                _receiverThread.Abort();
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                _connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void SetupReceive(Socket client)
        {
            _receiverThread = new Thread(Receive);
            _receiverThread.Start(client);
        }

        private static void Send(Socket client, string data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data + "\r\n");

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                var client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                _sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Receive(object obj)
        {
            try
            {
                var client = (Socket)obj;
                var state = new StateObject();
                state.WorkSocket = client;
                client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var state = (StateObject)ar.AsyncState;
                var client = state.WorkSocket;
                var bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    state.String.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
                }

                if (state.String.Length > 1)
                    Console.WriteLine(state.String.ToString());
                state.String = new StringBuilder();
                state.Buffer = new byte[StateObject.BufferSize];
                // continue receiving

                client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, SocketFlags.None,
                        new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
