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
    public partial class ClientForm : Form
    {
        CustomTS3Client _client;
        //channel name is unique, you cannot create two channel with same name, so can safe to use channel name as key to map channel ID
        Dictionary<string, Channel> _channels = new Dictionary<string,Channel>();
        Dictionary<string, ushort> _clients = new Dictionary<string, ushort>();
        public ClientForm()
        {
            InitializeComponent();

            _client = new CustomTS3Client();
            _client.NotificationNeeded += _client_NotificationNeeded;
            _client.ErrorOccured += _client_ErrorOccured;
            _client.TextMessage += _client_TextMessage;
            _client.NewChannel += _client_NewChannel;
            _client.ConnectStatusChange += _client_ConnectStatusChange;
            _client.ClientMove += _client_ClientMove;
            _client.ClientMoveTimeout += _client_ClientMoveTimeout;
        }

        void _client_ClientMoveTimeout(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID, int visibility, string timeoutMessage)
        {
            string clientName = _client.getStringVariable(clientID, ClientProperties.CLIENT_NICKNAME);
            _clients.Remove(clientName);
            removeFromClientList(clientName);
        }

        void _client_ClientMove(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID, int visibility, string moveMessage)
        {
            string clientName = _client.getStringVariable(clientID, ClientProperties.CLIENT_NICKNAME);
            //add him to client list
            if(newChannelID == _client.CurrentChannelID)
            {
                _clients.Add(clientName, clientID);
                addToClientList(clientName);
            }
                //remove client from list when he leaves
            else if(oldChannelID == _client.CurrentChannelID)
            {
                _clients.Remove(clientName);
                removeFromClientList(clientName);
            }
            clearClientSelection();
        }

        void _client_ConnectStatusChange(ulong serverID, ConnectStatus newStatus, uint errorNumber)
        {
            if(newStatus == ConnectStatus.STATUS_CONNECTION_ESTABLISHED)
            { 
                List<ushort> clients = _client.getChannelClientList();
                for (int i = 0; i < clients.Count; i++)
                {
                    string clientName = _client.getStringVariable(clients[i], ClientProperties.CLIENT_NICKNAME);
                    _clients.Add(clientName, clients[i]);
                    addToClientList(clientName);
                }
            }
        }
        
        void _client_NewChannel(ulong serverID, ulong channelID, ulong channelParentID)
        {
            string channelName = _client.getStringVariable(channelID, ChannelProperties.CHANNEL_NAME);
            if(channelName != string.Empty){
                Channel channel = new Channel();
                channel.ID = channelID;
                channel.name = channelName;
                channel.parentID = channelParentID;
                _channels.Add(channel.name, channel);
                addToChannelList(channel.name);
            }
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

        void _client_TextMessage(ulong serverID, TextMessageTargetMode targetMode, ushort toID, ushort fromID, string fromName, string fromUniqueIdentifier, string message)
        {
            //string msg = string.Format("[MSG|{0}|{1}|{2}|{3}|{4}]: {5}", serverID, targetMode.ToString(), toID, fromID, fromName, message);
            if (targetMode == TextMessageTargetMode.TextMessageTarget_CLIENT && _client.CurrentClientID == toID)
                message = string.Format("Whisper from {0}: {1}", fromName, message);
            else if (targetMode == TextMessageTargetMode.TextMessageTarget_CLIENT && _client.CurrentClientID == fromID)
                message = string.Format("Whisper to {0}: {1}", _client.getStringVariable(toID, ClientProperties.CLIENT_NICKNAME), message);
            updateTextbox(message, txtLog);
        }

        void _client_ErrorOccured(string message)
        {
            updateTextbox(message, txtLog);
        }

        private void _client_NotificationNeeded(string message)
        {
            updateTextbox(message, txtLog);
        }
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            _client.disconnect();
            listChannel.Items.Clear();
            _channels.Clear();
            listClient.Items.Clear();
            _clients.Clear();
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _client.disconnect();
            while (_client.CurrentState != ClientState.STATE_DISCONNECTED)
                Thread.Sleep(100);
            _client.kill();
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            _client.connect(txtNickname.Text, txtChannel.Text, string.Empty, txtIP.Text, txtPassword.Text);
        }
        public delegate void TextboxCallback(string message, TextBoxBase uiControl);
        public delegate void LabelCallback(string message, Label uiControl);

        public void updateLabel(string msg, Label uiControl)
        {
            if (this.InvokeRequired)
                Invoke(new LabelCallback(updateLabel), msg, uiControl);
            else
                uiControl.Text = msg;
        }
        public void updateTextbox(string msg, TextBoxBase uiControl)
        {
            if (this.InvokeRequired)
                Invoke(new TextboxCallback(updateTextbox), msg, uiControl);
            else
            {
                string message = DateTime.Now.ToString("[yyyyMMdd hh:mm:ss] ") + msg + Environment.NewLine;
                uiControl.AppendText(message);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string message = txtMsg.Text;
            if (message == string.Empty) return;

            bool result = false;
            if (rdbServer.Checked)
                result = _client.tell(message, TextMessageTargetMode.TextMessageTarget_SERVER);
            else if (rdbChannel.Checked)
                if (listChannel.SelectedItem == null)
                    result = _client.tell(message, TextMessageTargetMode.TextMessageTarget_CHANNEL, _client.CurrentChannelID);
                else
                    result = _client.tell(message, TextMessageTargetMode.TextMessageTarget_CHANNEL, _channels[listChannel.SelectedItem.ToString()].ID);
            else if (rdbClient.Checked)
                if (listClient.SelectedItem == null)
                    result = _client.tell(message, TextMessageTargetMode.TextMessageTarget_CLIENT, _previousSelectedClientID);
                else
                    result = _client.tell(message, TextMessageTargetMode.TextMessageTarget_CLIENT, _clients[listClient.SelectedItem.ToString()]);
            if (result) { 
                txtMsg.Clear();
                txtMsg.Focus();
            }
        }

        ushort _previousSelectedClientID = 0;
        private void listClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listClient.SelectedItem != null)
                _previousSelectedClientID = _clients[listClient.SelectedItem.ToString()];
        }

        private void btnMute_Click(object sender, EventArgs e)
        {
            if (listClient.SelectedItem == null)
                _client.localMute(_previousSelectedClientID);
            else
                _client.localMute(_clients[listClient.SelectedItem.ToString()]);
        }

        private void btnUnmute_Click(object sender, EventArgs e)
        {
            if (listClient.SelectedItem == null)
                _client.localUnmute(_previousSelectedClientID);
            else
                _client.localUnmute(_clients[listClient.SelectedItem.ToString()]);
        }


    }
    public class Channel
    {
        public ulong ID;
        public string name;
        public ulong parentID;
    }
}
