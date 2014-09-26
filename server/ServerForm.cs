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
using teamspeak.definitions;

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
            _server.ClientMoved += _server_ClientMoved;
            _server.ClientStartTalking += _server_ClientStartTalking;
            _server.ClientStopTalking += _server_ClientStopTalking;
            _server.ChannelCreated += _server_ChannelCreated;
            _server.ChannelDeleted += _server_ChannelDeleted;
        }

        void _server_ChannelDeleted(ulong serverID, ushort invokerClientID, ulong channelID)
        {
            string channelName = _server.getStringVariable(channelID, ChannelProperties.CHANNEL_NAME);
            string invokerName = _server.getStringVariable(invokerClientID, ClientProperties.CLIENT_NICKNAME);
            removeFromChannelList(channelName);
            updateText(string.Format("Channel {0} deleted by {1}", channelName, invokerName));
        }

        void _server_ChannelCreated(ulong serverID, ushort invokerClientID, ulong channelID)
        {
            string channelName = _server.getStringVariable(channelID, ChannelProperties.CHANNEL_NAME);
            string invokerName = _server.getStringVariable(invokerClientID, ClientProperties.CLIENT_NICKNAME);
            addToChannelList(channelName);
            updateText(string.Format("Channel {0} created by {1}", channelName, invokerName));
        }

        void _server_ClientStopTalking(ulong serverID, ushort clientID)
        {
            string clientName = _server.getStringVariable(clientID,ClientProperties.CLIENT_NICKNAME);
            if(listClient.Items.Contains(clientName+"*"))
                listClient.Items[listClient.Items.IndexOf(clientName+"*")] = clientName;
        }

        void _server_ClientStartTalking(ulong serverID, ushort clientID)
        {
            string clientName = _server.getStringVariable(clientID,ClientProperties.CLIENT_NICKNAME);
            if (listClient.Items.Contains(clientName))
                listClient.Items[listClient.Items.IndexOf(clientName)] = clientName += "*";
        }

        void _server_ClientMoved(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID)
        {
            string oldChannelName = _server.getStringVariable(oldChannelID, ChannelProperties.CHANNEL_NAME);
            string newChannelName = _server.getStringVariable(newChannelID, ChannelProperties.CHANNEL_NAME);
            string clientName = _server.getStringVariable(clientID, ClientProperties.CLIENT_NICKNAME);
            updateText(string.Format("{0} moved from channel {1} to channel {2}", clientName, oldChannelName, newChannelName));
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
            //updateText(string.Format("Client {0} disconnected from channel {1} on virtual server {2}",
            //     clientID, channelID, serverID));

            string clientName = _server.getStringVariable(clientID, ClientProperties.CLIENT_NICKNAME);
            removeFromClientList(clientName);
            updateText(string.Format("{0} disconnected from channel {1} on virtual server {2}",
                clientName, channelID, serverID));
        }

        void server_ClientConnected(ulong serverID, ushort clientID, ulong channelID, ref uint removeClientError)
        {
            string clientName = _server.getStringVariable(clientID, ClientProperties.CLIENT_NICKNAME);
            addToClientList(clientName);
            updateText(string.Format("{0} joined channel {1} on virtual server {2}",
                clientName, channelID, serverID));
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if(_server.start())
            {
                btnStart.Enabled = false;
                btnStop.Enabled = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_server.close()) { 
            btnStop.Enabled = false;
            btnStart.Enabled = true;
            }
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _server.close();
        }

        public void server_NotificationNeeded(string message)
        {
            updateText(message);
        }
        public delegate void defaultCallback();
        public delegate void AddTextToListCallback(string name);
        void clearClientSelection()
        {
            if (this.InvokeRequired)
                Invoke(new defaultCallback(clearClientSelection));
            else
                listClient.ClearSelected();
        }
        void removeFromClientList(string clientName)
        {
            if (this.InvokeRequired)
                Invoke(new AddTextToListCallback(removeFromClientList), clientName);
            else
                listClient.Items.Remove(clientName);
        }
        void addToClientList(string clientName)
        {
            if (this.InvokeRequired)
                Invoke(new AddTextToListCallback(addToClientList), clientName);
            else
                listClient.Items.Add(clientName);
        }
        void addToChannelList(string channelName)
        {
            if (this.InvokeRequired)
                Invoke(new AddTextToListCallback(addToChannelList), channelName);
            else
                listChannel.Items.Add(channelName);
        }
        void removeFromChannelList(string channelName)
        {
            if (this.InvokeRequired)
                Invoke(new AddTextToListCallback(addToChannelList), channelName);
            else
                listChannel.Items.Remove(channelName);
        }
        public delegate void setTextCallback(string message);

        public void updateText(string message)
        {
            if (this.InvokeRequired)
            {
                Invoke(new setTextCallback(updateText), message);
            }
            else
            {
                message = DateTime.Now.ToString("[yyyyMMdd hh:mm:ss] ") + message + Environment.NewLine;            
                txtMessage.AppendText(message);
            }
        }
    }
}
