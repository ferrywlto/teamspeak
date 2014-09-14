using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace teamspeak
{
    public partial class ServerForm : Form
    {
        ServerMainLoop serverLogic;

        public ServerForm()
        {
            InitializeComponent();

            serverLogic = new ServerMainLoop(this);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            serverLogic.startServer();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            serverLogic.closeServer();
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            serverLogic.closeServer();
        }

        public void writeLine(string msg)
        {
            txtMessage.AppendText(msg + Environment.NewLine);
        }
    }
}
