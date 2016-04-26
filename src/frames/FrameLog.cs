using System;
using System.Drawing;
using System.Windows.Forms;

namespace Core
{
    public partial class FrameLog : Form
    {
        public FrameLog()
        {
            InitializeComponent();

            GM.PrintEventHandler += cntMessageLog.AppendTextLog;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Text = GM.assemblyTitle + " " + GM.version + " " + GM.assemblyCopyright;
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            cntProgressBar.Value = 0;
            cntStatusLine.Text = "";

            GM.Messenger.Subscribe("PROGRESS", onSetProgressBar);
            GM.Messenger.Subscribe("STATUS", onSetStatusLine);
            GM.Messenger.Subscribe("TASK", onSetTask);
        }

        private void onSetProgressBar(ref object arg)
        {
            var v = (int)arg;
            Invoke((MethodInvoker)delegate { cntProgressBar.Value = v; });
        }

        private void onSetStatusLine(ref object arg)
        {
            var v = (string)arg;
            Invoke((MethodInvoker)delegate { cntStatusLine.Text = v; });
        }

        private void onSetTask(ref object arg)
        {
            var v = (bool)arg;
            Invoke((MethodInvoker)delegate { 
                //cntPause.Enabled = v;
                cntPause.Visible = v;
                cntProgressBar.Visible = v;
            });
        }

        private void FrameLog_Load(object sender, EventArgs e)
        {            
            GM.Print("Started ...");
        }

        private void cntPause_Click(object sender, EventArgs e)
        {
            if (cntPause.Text == "PAUSE") cntPause.Text = "UNPAUSE";
            else cntPause.Text = "PAUSE";

            GM.Messenger.Broadcast("PAUSE");
        }

        private void cntExit_Click(object sender, EventArgs e)
        {
            GM.Messenger.Broadcast("EXIT");
        }

        private void FrameLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing) return;
            e.Cancel = true;
            this.Hide();
        }

        private void FrameLog_FormClosed(object sender, FormClosedEventArgs e)
        {
            GM.PrintEventHandler -= cntMessageLog.AppendTextLog;
            GM.Messenger.Unsubscribe("PROGRESS");
            GM.Messenger.Unsubscribe("STATUS");
            GM.Messenger.Unsubscribe("TASK");
        }
    }
}
