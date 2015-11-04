using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jeffrey
{
    public class TCPClient
    {
        private string guidString { get; set; }
        private IPAddress ipAddress { get; set; }
        private int port { get; set; }
        private Socket clientSocket { get; set; }
        private byte[] data = new byte[4096];
        private int size = 4096;
        DateTime endTime { get; set; }
        DateTime startTime { get; set; }

        public TCPClient(String guidString, IPAddress ipAddress, int port, int counter)
        {
            this.port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
            this.ipAddress = ipAddress;
            this.guidString = guidString;
            this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.clientSocket.Connect(this.ipAddress, this.port);
        }

        public void StartSending()
        {

            //this.clientSocket.ConnectAsync(new SocketAsyncEventArgs());

            // Send the file name.

            startTime = DateTime.Now;

            //this.clientSocket.Send(Encoding.ASCII.GetBytes(guidString));

            var outputBuffer = Encoding.ASCII.GetBytes(guidString);

            this.clientSocket.BeginSend(outputBuffer, 0, outputBuffer.Length, SocketFlags.None,
                   new AsyncCallback(SendData), this.clientSocket);

            //// Receive the length of the filename.
            //byte[] data = new byte[4096];
            //clientSocket.Receive(data);
            ////TO DO: Change receiveng to be Async
            ////clientSocket.ReceiveAsync();
            //var endTime = DateTime.Now;

            //var received = Encoding.ASCII.GetChars(data).ToString();

            //if (guidString == received)
            //{
            //    Log.Error(String.Format("Send and received message ar not same. Sent: {0}, Received: {1}", guidString, received));
            //}

            //var result = endTime.Subtract(startTime);

            //Log.Info(result.ToString("h'h 'm'm 's's'"));


            //clientSocket.Close();
        }

        void SendData(IAsyncResult iar)
        {
            Socket remote = (Socket)iar.AsyncState;
            int sent = remote.EndSend(iar);
            remote.BeginReceive(data, 0, size, SocketFlags.None,
                          new AsyncCallback(ReceiveData), remote);
        }

        void ReceiveData(IAsyncResult iar)
        {
            Socket remote = (Socket)iar.AsyncState;
            int recv = remote.EndReceive(iar);
            string stringData = Encoding.ASCII.GetString(data, 0, recv);
            //results.Items.Add(stringData);

            var endTime = DateTime.Now;
            //clientSocket.Close();

            var received = Encoding.ASCII.GetChars(data).ToString();

            if (guidString == received)
            {
                Log.Error(String.Format("Send and received message ar not same. Sent: {0}, Received: {1}", guidString, received));
            }

            var result = endTime.Subtract(startTime);



            Log.Info(result.ToString("h'h 'm'm 's's'"));

        }

    }
}



