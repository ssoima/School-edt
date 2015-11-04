namespace Jeffrey
{
    partial class JeffreyUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.bSendRequest = new System.Windows.Forms.Button();
            this.nUDRequests = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nUDThreads = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.tBAddress = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tBPort = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.nUDSecond = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.cBRepeat = new System.Windows.Forms.CheckBox();
            this.timerRepeat = new System.Windows.Forms.Timer(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.bStopTimer = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nUDRequests)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDThreads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSecond)).BeginInit();
            this.SuspendLayout();
            // 
            // bSendRequest
            // 
            this.bSendRequest.Location = new System.Drawing.Point(300, 98);
            this.bSendRequest.Name = "bSendRequest";
            this.bSendRequest.Size = new System.Drawing.Size(122, 23);
            this.bSendRequest.TabIndex = 0;
            this.bSendRequest.Text = "Send requests";
            this.bSendRequest.UseVisualStyleBackColor = true;
            this.bSendRequest.Click += new System.EventHandler(this.bSendRequest_Click);
            // 
            // nUDRequests
            // 
            this.nUDRequests.Location = new System.Drawing.Point(118, 101);
            this.nUDRequests.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nUDRequests.Name = "nUDRequests";
            this.nUDRequests.Size = new System.Drawing.Size(120, 20);
            this.nUDRequests.TabIndex = 1;
            this.nUDRequests.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Numbe of requests: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Number of Threads: ";
            // 
            // nUDThreads
            // 
            this.nUDThreads.Location = new System.Drawing.Point(118, 128);
            this.nUDThreads.Name = "nUDThreads";
            this.nUDThreads.Size = new System.Drawing.Size(120, 20);
            this.nUDThreads.TabIndex = 5;
            this.nUDThreads.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "IP address: ";
            // 
            // tBAddress
            // 
            this.tBAddress.Location = new System.Drawing.Point(118, 13);
            this.tBAddress.Name = "tBAddress";
            this.tBAddress.Size = new System.Drawing.Size(185, 20);
            this.tBAddress.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(370, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Port: ";
            // 
            // tBPort
            // 
            this.tBPort.Location = new System.Drawing.Point(408, 12);
            this.tBPort.Name = "tBPort";
            this.tBPort.Size = new System.Drawing.Size(100, 20);
            this.tBPort.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 179);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Repeat every ";
            // 
            // nUDSecond
            // 
            this.nUDSecond.Location = new System.Drawing.Point(118, 177);
            this.nUDSecond.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUDSecond.Name = "nUDSecond";
            this.nUDSecond.Size = new System.Drawing.Size(120, 20);
            this.nUDSecond.TabIndex = 11;
            this.nUDSecond.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(244, 179);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(12, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "s";
            // 
            // cBRepeat
            // 
            this.cBRepeat.AutoSize = true;
            this.cBRepeat.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cBRepeat.Location = new System.Drawing.Point(12, 154);
            this.cBRepeat.Name = "cBRepeat";
            this.cBRepeat.Size = new System.Drawing.Size(125, 17);
            this.cBRepeat.TabIndex = 14;
            this.cBRepeat.Text = "Repeat sending data";
            this.cBRepeat.UseVisualStyleBackColor = true;
            this.cBRepeat.CheckedChanged += new System.EventHandler(this.cBRepeat_CheckedChanged);
            // 
            // timerRepeat
            // 
            this.timerRepeat.Tick += new System.EventHandler(this.timerRepeat_Tick);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 241);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Repeated 0 times";
            // 
            // bStopTimer
            // 
            this.bStopTimer.Location = new System.Drawing.Point(300, 236);
            this.bStopTimer.Name = "bStopTimer";
            this.bStopTimer.Size = new System.Drawing.Size(122, 23);
            this.bStopTimer.TabIndex = 16;
            this.bStopTimer.Text = "Stop Timer";
            this.bStopTimer.UseVisualStyleBackColor = true;
            this.bStopTimer.Click += new System.EventHandler(this.bStopTimer_Click);
            // 
            // JeffreyUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSalmon;
            this.ClientSize = new System.Drawing.Size(605, 333);
            this.Controls.Add(this.bStopTimer);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cBRepeat);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.nUDSecond);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tBPort);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tBAddress);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nUDThreads);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nUDRequests);
            this.Controls.Add(this.bSendRequest);
            this.MaximizeBox = false;
            this.Name = "JeffreyUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Jeffrey";
            ((System.ComponentModel.ISupportInitialize)(this.nUDRequests)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDThreads)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUDSecond)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bSendRequest;
        private System.Windows.Forms.NumericUpDown nUDRequests;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nUDThreads;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tBAddress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tBPort;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nUDSecond;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox cBRepeat;
        private System.Windows.Forms.Timer timerRepeat;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button bStopTimer;
    }
}

