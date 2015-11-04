using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;

namespace PickByLightSimulator
{
    static class Program
    {
        private const int DefaultPort = 13000;
        private static Logger Log = LogManager.GetCurrentClassLogger();
        private static TcpListener _server = null;
        private static Form1 _form = null;
        private static bool _handlersInstalled = false;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _form = new Form1();
            int port = GetPortFromConfig();
            _form.Text = string.Format("Pick by Light Simulator - Serving on IP: *:{0}", port);
            _server = new TcpListener(IPAddress.Any, port);
            _server.Start();
            // configure our listener thread
            var th = new Thread(new ThreadStart(Run));
            th.Start();
            Application.Run(_form);
            th.Abort();
            _server.Stop();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private static void Run()
        {
            while (true)
            {
                while (!_server.Pending())
                {
                    Thread.Sleep(500);
                }
                var client = _server.AcceptTcpClient();
                
                try
                {
                    using (var stream = client.GetStream())
                    using (var reader = new StreamReader(stream))
                    using (var writer = new StreamWriter(stream))
                    {
                        Action<string> handler = command =>
                        {
                            var ackCommand = "ACK:" + command;
                            Log.Debug("sending: " + ackCommand);
                            writer.WriteLine(ackCommand);
                            writer.Flush();
                        };
                        try
                        {
                            _form.ButtonClicked += handler;

                            // and we are going to wait for commands
                            while (true)
                            {
                                var data = reader.ReadLine();
                                Log.Debug("received: " + data);
                                // notify form
                                NotifyForm(data);
                            }
                        }
                        finally
                        {
                            _form.ButtonClicked -= handler;
                        }
                    }
                }
                catch (ObjectDisposedException ode)
                {
                    // could be closed already
                    Log.Error("Object disposed: " + ode.Message, ode);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message, ex);
                }
                client.Close();
            }
        }

        private static void NotifyForm(string message)
        {
            // parse message for row:column info
            var info = message.Split(':');
            if (info.Length != 2) throw new InvalidOperationException("Invalid command");
            var row = 0;
            var col = 0;
            if (!Int32.TryParse(info[0], out row)) throw new InvalidOperationException("Not a valid row number");
            if (!Int32.TryParse(info[1], out col)) throw new InvalidOperationException("Not a valid col number");

            var tbl = _form.Controls["layoutContainer"] as TableLayoutPanel;
            if (tbl == null)
            {
                Log.Error("No table layout panel available");
                return;
            }
            var button = tbl.GetControlFromPosition(col - 1, row - 1) as Button;
            if (button == null)
            {
                Log.Error("Could not get button at position " + message);
                return;
            }
            button.Invoke(new Action(delegate { button.BackColor = SystemColors.Highlight; }));
        }

        private static int GetPortFromConfig()
        {
            int port;
            if (!Int32.TryParse(ConfigurationManager.AppSettings["port"], out port)) return DefaultPort;
            return port;
        }
    }
}
