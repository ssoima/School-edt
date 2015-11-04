using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Brandt
{
    public class TCPSocketListener
    {
        Socket clientSocket { get; set; }
        Thread clientListenerThread = null;
        DateTime lastReceiveDateTime { get; set; }
        DateTime currentReceiveDateTime { get; set; }
        bool stopClient { get; set; }

        public TCPSocketListener(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
        }

        public void StartSocketListener()
        {
            if (this.clientSocket != null)
            {
                clientListenerThread = new Thread(new ThreadStart(SocketListenerThreadStart));

                clientListenerThread.Start();
            }
        }

        private void SocketListenerThreadStart()
        {
            int size = 0;
            Byte[] byteBuffer = new Byte[1024];

            lastReceiveDateTime = DateTime.Now;
            currentReceiveDateTime = DateTime.Now;

            //Timer t = new Timer(new TimerCallback(CheckClientCommInterval),
            //  null, 15000, 15000);

            while (!stopClient)
            {
                try
                {
                    size = this.clientSocket.Receive(byteBuffer);
                    currentReceiveDateTime = DateTime.Now;

                    this.clientSocket.Send(byteBuffer);
                    //ParseReceiveBuffer(byteBuffer, size);
                }
                catch (SocketException se)
                {
                    stopClient = true;
                    //markedForDeletion = true;
                }
            }
            //t.Change(Timeout.Infinite, Timeout.Infinite);
            //t = null;
        }
    }
}
