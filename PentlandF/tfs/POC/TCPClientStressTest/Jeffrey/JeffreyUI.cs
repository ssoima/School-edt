using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jeffrey
{
    public partial class JeffreyUI : Form
    {
        int port { get; set; }
        string ip { get; set; }
        List<string> guids { get; set; }
        static int countClients { get; set; }
        string lockCounter { get; set; }
        int timerCount { get; set; }
        List<TCPClient> clients { get; set; }

        public JeffreyUI()
        {
            InitializeComponent();
            this.lockCounter = "LockCounter";
            tBPort.Text = ConfigurationManager.AppSettings["port"].ToString();
            tBAddress.Text = ConfigurationManager.AppSettings["ip"].ToString();
            EnableRepeatSending(false);
            guids = new List<string>();
            for (int i = 0; i < 16000; i++)
            {
                guids.Add(String.Format("{0}<EOF>", Guid.NewGuid().ToString()));
            }
        }

        private void bSendRequest_Click(object sender, EventArgs e)
        {
            Log.Info("******************  Start process ******************");
            countClients = 0;
            timerCount = 0;
            //guids = new List<string>();
            //for (int i = 0; i < nUDRequests.Value; i++)
            //{
            //    guids.Add(String.Format("{0}<EOF>", Guid.NewGuid().ToString()));
            //}

            port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
            ip = ConfigurationManager.AppSettings["ip"].ToString();

            Log.Info("******************  create clients  ******************");
            clients = new List<TCPClient>();
            for (int i = 0; i < nUDRequests.Value; i++)
            {
                // new TCPClient(guids[i], IPAddress.Parse(ip), port);
                clients.Add(new TCPClient(guids[i], IPAddress.Parse(tBAddress.Text), Convert.ToInt32(tBPort.Text), i));
                IncrementClientCounter();
            }

            for (int i = 0; i < nUDThreads.Value; i++)
            {
                var t = new Thread(new ThreadStart(StartSending));
                t.Start();
            }

            if (cBRepeat.Checked)
            {
                timerRepeat.Interval = Convert.ToInt32(nUDSecond.Value) * 1000;
                timerRepeat.Start();
            }
        }

        private void StartSending()
        {


            Log.Info("******************  start sending  ******************");
            clients.ForEach(m => m.StartSending());
            Log.Info("******************  end inizialization  ******************");
        }

        private void IncrementClientCounter()
        {
            lock (this.lockCounter)
            {
                countClients++;
                //Log.Info(String.Format("Number of clients: {0}", countClients));
            }
        }

        private void cBRepeat_CheckedChanged(object sender, EventArgs e)
        {
            if (cBRepeat.Checked)
            {
                EnableRepeatSending(true);
            }
            else
            {
                EnableRepeatSending(false);
            }
        }

        private void EnableRepeatSending(bool toRepeat)
        {
            label5.Enabled = toRepeat;
            label6.Enabled = toRepeat;
            nUDSecond.Enabled = toRepeat;
        }

        private void timerRepeat_Tick(object sender, EventArgs e)
        {
            timerCount++;
            label7.Text = String.Format("Repeated {0} times", timerCount);
            for (int i = 0; i < nUDThreads.Value; i++)
            {
                var t = new Thread(new ThreadStart(StartSending));
                t.Start();
            }
        }

        private void bStopTimer_Click(object sender, EventArgs e)
        {
            timerRepeat.Stop();
        }

    }
}
