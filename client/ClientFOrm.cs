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
    public class Channel
    {
        public ulong ID;
        public string name;
        public ulong parentID;
    }
    public partial class ClientForm : Form
    {
        CustomTS3Client _client;
        //channel name is unique, you cannot create two channel with same name, so can safe to use channel name as key to map channel ID
        Dictionary<string, Channel> _channels = new Dictionary<string,Channel>();

        public ClientForm()
        {
            InitializeComponent();

            _client = new CustomTS3Client();
            _client.NotificationNeeded += _client_NotificationNeeded;
            _client.ErrorOccured += _client_ErrorOccured;
            _client.ConnectStatusChange += _client_ConnectStatusChange;
            _client.TextMessage += _client_TextMessage;
            _client.NewChannel += _client_NewChannel;

        }
        
        void _client_NewChannel(ulong serverID, ulong channelID, ulong channelParentID)
        {
            string channelName = _client.getStringVariable(channelID, ChannelProperties.CHANNEL_NAME);
            if(channelName != string.Empty){
                Channel channel = new Channel();
                channel.ID = channelID;
                channel.name = channelName;
                channel.parentID = channelParentID;

                addToChannelList(channel);
            }
        }
        public delegate void AddChannelListCallback(Channel channel);
        void addToChannelList(Channel channel)
        {
            if (this.InvokeRequired)
                Invoke(new AddChannelListCallback(addToChannelList), channel);
            else
            {
                _channels.Add(channel.name, channel);
                listChannel.Items.Add(channel.name);
            }
        }
        void _client_TextMessage(ulong serverID, TextMessageTargetMode targetMode, ushort toID, ushort fromID, string fromName, string fromUniqueIdentifier, string message)
        {
            string msg = string.Format("[MSG|{0}|{1}|{2}|{3}|{4}]: {5}", serverID, targetMode.ToString(), toID, fromID, fromName, message);
            updateTextbox(msg, txtLog);
        }

        void _client_ConnectStatusChange(ulong serverID, ConnectStatus newStatus, uint errorNumber)
        {
            updateLabel(newStatus.ToString(), lblConnStatus);
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

        private void txtIP_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNickname_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblConnStatus_Click(object sender, EventArgs e)
        {

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            bool result = false;
            if (rdbServer.Checked)
                result = _client.tell(txtMsg.Text, TextMessageTargetMode.TextMessageTarget_SERVER);
            else if (rdbChannel.Checked)
                result = _client.tell(txtMsg.Text, TextMessageTargetMode.TextMessageTarget_CHANNEL, 0);
            else if (rdbClient.Checked)
                result = _client.tell(txtMsg.Text, TextMessageTargetMode.TextMessageTarget_CLIENT, 0);
            if (result)
                txtMsg.Clear();
        }


    }
}
