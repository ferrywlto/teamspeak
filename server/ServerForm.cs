using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace teamspeak
{
    public partial class ServerForm : Form
    {
        TS3CustomServer _server;

        public ServerForm()
        {
            InitializeComponent();

            _server = new TS3CustomServerEx();
            _server.NotificationNeeded += server_NotificationNeeded;
            _server.ClientConnected += server_ClientConnected;
            _server.ClientDisconnected += server_ClientDisconnected;
            _server.ChannelTextMessage += server_ChannelTextMessage;
            _server.ServerTextMessage += server_ServerTextMessage;
        }

        void server_ServerTextMessage(ulong serverID, ushort invokerClientID, string textMessage)
        {
            updateText(string.Format("[Server Message][Server: {0}][Client: {1}]:  {2}", serverID, invokerClientID, textMessage));
        }

        void server_ChannelTextMessage(ulong serverID, ushort invokerClientID, ulong targetChannelID, string textMessage)
        {
            updateText(string.Format("[Server: {0}][Channel: {1}][Client: {2}]:  {3}", serverID, targetChannelID, invokerClientID, textMessage));
        }

        void server_ClientDisconnected(ulong serverID, ushort clientID, ulong channelID)
        {
            updateText(string.Format("Client {0} disconnected from channel {1} on virtual server {2}",
                 clientID, channelID, serverID));
        }

        void server_ClientConnected(ulong serverID, ushort clientID, ulong channelID, ref uint removeClientError)
        {
            updateText(string.Format("Client {0} joined channel {1} on virtual server {2}",
                clientID, channelID, serverID));
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _server.start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _server.close();
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _server.close();
        }

        public void server_NotificationNeeded(string message)
        {
            updateText(message);
        }

        public delegate void setTextCallback(string message);

        public void updateText(string message)
        {
            message = DateTime.Now.ToString("[yyyyMMdd hh:mm:ss] ") + message + Environment.NewLine;
            if (this.InvokeRequired)
            {
                Invoke(new setTextCallback(updateText), message);
            }
            else
                txtMessage.AppendText(message);
        }
    }
}
