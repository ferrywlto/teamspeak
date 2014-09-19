namespace teamspeak
{
    partial class ClientForm
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
            this.btnSend = new System.Windows.Forms.Button();
            this.txtMsg = new System.Windows.Forms.TextBox();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtNickname = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.lblConnStatus = new System.Windows.Forms.Label();
            this.listClient = new System.Windows.Forms.ListBox();
            this.listChannel = new System.Windows.Forms.ListBox();
            this.rdbClient = new System.Windows.Forms.RadioButton();
            this.rdbChannel = new System.Windows.Forms.RadioButton();
            this.rdbServer = new System.Windows.Forms.RadioButton();
            this.txtChannel = new System.Windows.Forms.TextBox();
            this.txtChannelPassword = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(526, 465);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(87, 31);
            this.btnSend.TabIndex = 0;
            this.btnSend.Text = "send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtMsg
            // 
            this.txtMsg.Location = new System.Drawing.Point(0, 469);
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.Size = new System.Drawing.Size(520, 22);
            this.txtMsg.TabIndex = 1;
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(410, 6);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(101, 22);
            this.txtIP.TabIndex = 2;
            this.txtIP.Text = "10.0.1.3";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(410, 34);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(101, 22);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.Text = "secret";
            // 
            // txtNickname
            // 
            this.txtNickname.Location = new System.Drawing.Point(410, 62);
            this.txtNickname.Name = "txtNickname";
            this.txtNickname.Size = new System.Drawing.Size(101, 22);
            this.txtNickname.TabIndex = 4;
            this.txtNickname.Text = "ferry";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(526, 34);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(87, 23);
            this.btnConnect.TabIndex = 5;
            this.btnConnect.Text = "connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(0, 255);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(613, 204);
            this.txtLog.TabIndex = 6;
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(526, 62);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(87, 23);
            this.btnDisconnect.TabIndex = 7;
            this.btnDisconnect.Text = "disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // lblConnStatus
            // 
            this.lblConnStatus.AutoSize = true;
            this.lblConnStatus.Location = new System.Drawing.Point(411, 145);
            this.lblConnStatus.Name = "lblConnStatus";
            this.lblConnStatus.Size = new System.Drawing.Size(109, 17);
            this.lblConnStatus.TabIndex = 8;
            this.lblConnStatus.Text = "Nothing happen";
            // 
            // listClient
            // 
            this.listClient.FormattingEnabled = true;
            this.listClient.ItemHeight = 16;
            this.listClient.Location = new System.Drawing.Point(0, 6);
            this.listClient.Name = "listClient";
            this.listClient.Size = new System.Drawing.Size(192, 244);
            this.listClient.TabIndex = 9;
            // 
            // listChannel
            // 
            this.listChannel.FormattingEnabled = true;
            this.listChannel.ItemHeight = 16;
            this.listChannel.Location = new System.Drawing.Point(198, 6);
            this.listChannel.Name = "listChannel";
            this.listChannel.Size = new System.Drawing.Size(206, 244);
            this.listChannel.TabIndex = 10;
            // 
            // rdbClient
            // 
            this.rdbClient.AutoSize = true;
            this.rdbClient.Checked = true;
            this.rdbClient.Location = new System.Drawing.Point(410, 174);
            this.rdbClient.Name = "rdbClient";
            this.rdbClient.Size = new System.Drawing.Size(64, 21);
            this.rdbClient.TabIndex = 11;
            this.rdbClient.TabStop = true;
            this.rdbClient.Text = "Client";
            this.rdbClient.UseVisualStyleBackColor = true;
            // 
            // rdbChannel
            // 
            this.rdbChannel.AutoSize = true;
            this.rdbChannel.Location = new System.Drawing.Point(410, 202);
            this.rdbChannel.Name = "rdbChannel";
            this.rdbChannel.Size = new System.Drawing.Size(81, 21);
            this.rdbChannel.TabIndex = 12;
            this.rdbChannel.Text = "Channel";
            this.rdbChannel.UseVisualStyleBackColor = true;
            // 
            // rdbServer
            // 
            this.rdbServer.AutoSize = true;
            this.rdbServer.Location = new System.Drawing.Point(410, 230);
            this.rdbServer.Name = "rdbServer";
            this.rdbServer.Size = new System.Drawing.Size(71, 21);
            this.rdbServer.TabIndex = 13;
            this.rdbServer.Text = "Server";
            this.rdbServer.UseVisualStyleBackColor = true;
            // 
            // txtChannel
            // 
            this.txtChannel.Location = new System.Drawing.Point(411, 91);
            this.txtChannel.Name = "txtChannel";
            this.txtChannel.Size = new System.Drawing.Size(100, 22);
            this.txtChannel.TabIndex = 14;
            this.txtChannel.Text = "D3";
            // 
            // txtChannelPassword
            // 
            this.txtChannelPassword.Location = new System.Drawing.Point(411, 120);
            this.txtChannelPassword.Name = "txtChannelPassword";
            this.txtChannelPassword.Size = new System.Drawing.Size(100, 22);
            this.txtChannelPassword.TabIndex = 15;
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 501);
            this.Controls.Add(this.txtChannelPassword);
            this.Controls.Add(this.txtChannel);
            this.Controls.Add(this.rdbServer);
            this.Controls.Add(this.rdbChannel);
            this.Controls.Add(this.rdbClient);
            this.Controls.Add(this.listChannel);
            this.Controls.Add(this.listClient);
            this.Controls.Add(this.lblConnStatus);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtNickname);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtIP);
            this.Controls.Add(this.txtMsg);
            this.Controls.Add(this.btnSend);
            this.Name = "ClientForm";
            this.Text = "Client Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtMsg;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtNickname;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Label lblConnStatus;
        private System.Windows.Forms.ListBox listClient;
        private System.Windows.Forms.ListBox listChannel;
        private System.Windows.Forms.RadioButton rdbClient;
        private System.Windows.Forms.RadioButton rdbChannel;
        private System.Windows.Forms.RadioButton rdbServer;
        private System.Windows.Forms.TextBox txtChannel;
        private System.Windows.Forms.TextBox txtChannelPassword;
    }
}