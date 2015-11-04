using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PbLTest
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private string receiveBuffer = "";
        private char sotByte = Convert.ToChar(2);
        private char eotByte = Convert.ToChar(3);

        public Form1()
        {
            InitializeComponent();
            client = new TcpClient();
            client.DataReceived += client_DataReceived;

            comboBoxCommand.Items.AddRange(Enum.GetNames(typeof(PblCmd)));
            comboBoxCommand.SelectedIndex = 0;
            comboBoxColor.Items.AddRange(Enum.GetNames(typeof(PblColor)));
            comboBoxColor.SelectedIndex = 0;
        }

        void client_DataReceived(object sender, DataReceviedEventArgs e)
        {
            // WinForm stuff, as this event is called from a different Thread in order to be able to perform UI changes
            if (this.InvokeRequired)
                this.Invoke(new EventHandler<DataReceviedEventArgs>(client_DataReceived), sender, e);
            else
            {
                textBoxReceive.Text += "Received: " + e.ReceivedString + Environment.NewLine;

                // Add all incoming data to a buffer because this event could be called multiple times for a message.
                receiveBuffer += e.ReceivedString;


                // Progress as long the buffer contains a message (start with '<' ends with '>')
                var msgStartIndex = receiveBuffer.IndexOf(sotByte);
                if (msgStartIndex > -1)
                    receiveBuffer = receiveBuffer.Substring(msgStartIndex); // Crop crap (text/data between messages)
                var msgEndIndex = receiveBuffer.IndexOf(eotByte);

                while (receiveBuffer.Length > 0 && receiveBuffer[0] == sotByte && msgEndIndex > -1)
                {
                    var message = receiveBuffer.Substring(1, msgEndIndex - 1);
                    receiveBuffer = receiveBuffer.Substring(msgEndIndex + 1);

                    PblMessage m = JsonConvert.DeserializeObject<PblMessage>(message);
                    processMessage(m);

                    msgStartIndex = receiveBuffer.IndexOf(sotByte);
                    if (msgStartIndex > -1)
                        receiveBuffer = receiveBuffer.Substring(msgStartIndex);
                    msgEndIndex = receiveBuffer.IndexOf(eotByte);
                }
            }

        }

        private void processMessage(PblMessage message)
        {
            if (message.cmd == PblCmd.Button)
            {
                textBoxReceive.Text += "Button pressed at Slave: " + message.sid.ToString() + Environment.NewLine;
            }
        }

        private void buttonReceive_Click(object sender, EventArgs e)
        {
            client.Connect(textHost.Text, 23);
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            PblMessage msg = new PblMessage();
            msg.id = (int)numericSlaveId.Value;
            msg.cmd = (PblCmd)Enum.Parse(typeof(PblCmd), comboBoxCommand.Text);
            msg.color = (PblColor)Enum.Parse(typeof(PblColor), comboBoxColor.Text);
            msg.value = textBoxValue.Text;

            string json = JsonConvert.SerializeObject(msg);
            client.Send(sotByte + json + eotByte);
        }


    }
}
