using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace Core
{
    public class Mainframe : ApplicationContext
    {
        struct balloonMessage
        {
            public int          timeout;
            public string       title;
            public string       message;
            public ToolTipIcon  icon;
            public balloonMessage(int timeout, string title, string message, ToolTipIcon icon) 
            {
                this.timeout    = timeout;
                this.title      = title;
                this.message    = message;
                this.icon       = icon;
            }
        }
        private System.Timers.Timer balloonTimer = new System.Timers.Timer();
        private Queue<balloonMessage> balloonQueue = new Queue<balloonMessage>();

        private readonly NotifyIcon notifyIcon;
        
        FrameSelectTask frameSelectTask = new FrameSelectTask();
        FrameLog frameLog = new FrameLog();

        private Thread taskThread;

        public Mainframe()
        {   
            notifyIcon = new NotifyIcon();
            notifyIcon.Text = GM.assemblyTitle + " " + GM.version + " " + GM.assemblyCopyright;
            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon.DoubleClick += new EventHandler(onDoubleClick);
            notifyIcon.ContextMenu = new ContextMenu();
            notifyIcon.ContextMenu.MenuItems.Add(new MenuItem("Exit", new EventHandler(onMenuExit)));
            notifyIcon.Visible = true;

            if (Control.ModifierKeys == Keys.Shift) frameSelectTask.ShowDialog();

            var h = frameLog.Handle; // create all form controls
            if (Task.mode != "0") frameLog.Show();            

            taskThread = new Thread(new ThreadStart(Task.DoTask)); //taskThread = new Thread(delegate() { Task.DoTask(); });
            taskThread.IsBackground = true;
            taskThread.Start();

            GM.Log("Task thread started ...");

            balloonTimer.Elapsed += onBalloonTimerTick;
            balloonTimer.AutoReset = false;

            GM.Messenger.Subscribe("BALLOON", onShowBalloonTip);
            GM.Messenger.Subscribe("PAUSE", Task.onPause);
            GM.Messenger.Subscribe("EXIT", Task.onExit);            
            GM.Messenger.Subscribe("EXIT", onExit);
        }

        void onBalloonTimerTick(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (balloonQueue.Count == 0) return;
            
            var bm = balloonQueue.Dequeue();
            balloonTimer.Interval = bm.timeout;
            balloonTimer.Enabled = true;
            notifyIcon.ShowBalloonTip(bm.timeout, bm.title, bm.message, bm.icon);
            if (Task.mode == "0") Thread.Sleep(bm.timeout);
        }

        public void onShowBalloonTip(ref object arg)
        {
            var a = arg as List<object>;
            balloonQueue.Enqueue(new balloonMessage((int)((TimeSpan)a[0]).TotalMilliseconds, (string)a[1], (string)a[2], (ToolTipIcon)a[3]));
            if(!balloonTimer.Enabled) onBalloonTimerTick(null, null);
        }

        public void onDoubleClick(object sender, EventArgs e)
        {            
            if (frameLog.WindowState == FormWindowState.Minimized) frameLog.WindowState = FormWindowState.Normal;
            if (!frameLog.Visible) frameLog.Show();
            frameLog.Activate();            
        }

        public void onMenuExit(object sender, EventArgs e)
        {
            doExit();
        }

        public void onExit(ref object arg)
        {
            GM.DelayedCall(doExit, 100);
        }

        public void doExit()
        {
            while (taskThread.IsAlive) Thread.Sleep(0); // taskThread.Join();
            
            GM.Messenger.Unsubscribe("BALLOON");
            GM.Messenger.Unsubscribe("PAUSE");
            GM.Messenger.Unsubscribe("EXIT");
            
            notifyIcon.Visible = false;            
            Application.Exit();
        }
    }
}
