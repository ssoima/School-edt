using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Brandt
{
    public class TCPServer
    {
        TcpListener server = null;
        List<TCPSocketListener> socketListenersList = null;
        Thread serverThread = null;
        Thread purgingThread = null;
        bool stopServer { get; set; }
        bool stopPurging { get; set; }

        public TCPServer()
        {
            var port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
            var ip = ConfigurationManager.AppSettings["ip"].ToString();
            var address = IPAddress.Parse(ip);
            server = new TcpListener(address, port);
        }

        public void StartServer()
        {
            if (server != null)
            {
                // Create a ArrayList for storing SocketListeners before
                // starting the server.
                socketListenersList = new List<TCPSocketListener>();

                // Start the Server and start the thread to listen client 
                // requests.
                server.Start();
                serverThread = new Thread(new ThreadStart(ServerThreadStart));
                serverThread.Start();
            }
        }

        private void ServerThreadStart()
        {
            // Client Socket variable;
            Socket clientSocket = null;
            TCPSocketListener socketListener = null;
            while (!stopServer)
            {
                try
                {
                    // Wait for any client requests and if there is any 
                    // request from any client accept it (Wait indefinitely).
                    clientSocket = server.AcceptSocket();

                    // Create a SocketListener object for the client.
                    socketListener = new TCPSocketListener(clientSocket);

                    // Add the socket listener to an array list in a thread 
                    // safe fashon.
                    //Monitor.Enter(m_socketListenersList);
                    lock (socketListenersList)
                    {
                        socketListenersList.Add(socketListener);
                    }
                    //Monitor.Exit(m_socketListenersList);

                    // Start a communicating with the client in a different
                    // thread.
                    socketListener.StartSocketListener();
                }
                catch (SocketException se)
                {
                    stopServer = true;
                }
            }
        }

        public void StopServer()
        {
            if (server != null)
            {
                // It is important to Stop the server first before doing
                // any cleanup. If not so, clients might being added as
                // server is running, but supporting data structures
                // (such as m_socketListenersList) are cleared. This might
                // cause exceptions.

                // Stop the TCP/IP Server.
                stopServer = true;
                server.Stop();

                // Wait for one second for the the thread to stop.
                serverThread.Join(1000);

                // If still alive; Get rid of the thread.
                if (serverThread.IsAlive)
                {
                    serverThread.Abort();
                }
                serverThread = null;

                stopPurging = true;
                //purgingThread.Join(1000);
                //if (purgingThread.IsAlive)
                //{
                //    purgingThread.Abort();
                //}
                purgingThread = null;

                // Free Server Object.
                server = null;

                // Stop All clients.
                //StopAllSocketListers();
            }
        }
    }
}