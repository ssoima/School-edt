using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Brandt
{
    public partial class BrandtUI : Form
    {
        //TCPServer server { get; set; }
        //AsynchronousServer server { get; set; }
        Server server { get; set; }
        public BrandtUI()
        {
            InitializeComponent();
            lIpAddress.Text = String.Format("{0}:{1}", ConfigurationManager.AppSettings["ip"].ToString(), ConfigurationManager.AppSettings["port"].ToString());
            HandleRequests();
        }

        public void HandleRequests()
        {
            //server = new TCPServer();
            //server.StartServer();
            server = new Server();
            //AsynchronousServer.StartListening();            

        }

        private void BrandtUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (null != server)
            {
                //server.StopServer();
                server = null;
            }
        }

        //private void bStart_Click(object sender, EventArgs e)
        //{
        //    bStart.Text = "Listening...";
        //    HandleRequests();
        //}

    }
}
