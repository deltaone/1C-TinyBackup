namespace Core
{
    partial class FrameLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrameLog));
            this.cntStatusStrip = new System.Windows.Forms.StatusStrip();
            this.cntProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.cntStatusLine = new System.Windows.Forms.ToolStripStatusLabel();
            this.cntPause = new System.Windows.Forms.Button();
            this.cntExit = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cntMessageLog = new ControlsEx.RichTextLog();
            this.cntStatusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // cntStatusStrip
            // 
            this.cntStatusStrip.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cntStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cntProgressBar,
            this.cntStatusLine});
            this.cntStatusStrip.Location = new System.Drawing.Point(0, 150);
            this.cntStatusStrip.Name = "cntStatusStrip";
            this.cntStatusStrip.Size = new System.Drawing.Size(794, 22);
            this.cntStatusStrip.SizingGrip = false;
            this.cntStatusStrip.TabIndex = 1;
            this.cntStatusStrip.Text = "statusStrip1";
            // 
            // cntProgressBar
            // 
            this.cntProgressBar.Name = "cntProgressBar";
            this.cntProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // cntStatusLine
            // 
            this.cntStatusLine.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cntStatusLine.Name = "cntStatusLine";
            this.cntStatusLine.Size = new System.Drawing.Size(30, 17);
            this.cntStatusLine.Text = "[xxx]";
            // 
            // cntPause
            // 
            this.cntPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cntPause.Location = new System.Drawing.Point(679, 92);
            this.cntPause.Name = "cntPause";
            this.cntPause.Size = new System.Drawing.Size(109, 23);
            this.cntPause.TabIndex = 2;
            this.cntPause.Text = "PAUSE";
            this.cntPause.UseVisualStyleBackColor = true;
            this.cntPause.Click += new System.EventHandler(this.cntPause_Click);
            // 
            // cntExit
            // 
            this.cntExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cntExit.Location = new System.Drawing.Point(679, 120);
            this.cntExit.Name = "cntExit";
            this.cntExit.Size = new System.Drawing.Size(109, 23);
            this.cntExit.TabIndex = 3;
            this.cntExit.Text = "EXIT";
            this.cntExit.UseVisualStyleBackColor = true;
            this.cntExit.Click += new System.EventHandler(this.cntExit_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(696, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(79, 75);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // cntMessageLog
            // 
            this.cntMessageLog.BackColor = System.Drawing.SystemColors.ControlLight;
            this.cntMessageLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cntMessageLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.cntMessageLog.ForeColor = System.Drawing.Color.Blue;
            this.cntMessageLog.Location = new System.Drawing.Point(-2, 0);
            this.cntMessageLog.LogMaxLines = 150;
            this.cntMessageLog.Name = "cntMessageLog";
            this.cntMessageLog.ReadOnly = true;
            this.cntMessageLog.Size = new System.Drawing.Size(675, 147);
            this.cntMessageLog.TabIndex = 0;
            this.cntMessageLog.TabStop = false;
            this.cntMessageLog.Text = "";
            // 
            // FrameLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 172);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.cntExit);
            this.Controls.Add(this.cntPause);
            this.Controls.Add(this.cntStatusStrip);
            this.Controls.Add(this.cntMessageLog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "FrameLog";
            this.Text = "FrameLog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrameLog_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrameLog_FormClosed);
            this.Load += new System.EventHandler(this.FrameLog_Load);
            this.cntStatusStrip.ResumeLayout(false);
            this.cntStatusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ControlsEx.RichTextLog cntMessageLog;
        public System.Windows.Forms.ToolStripProgressBar cntProgressBar;
        protected System.Windows.Forms.StatusStrip cntStatusStrip;
        public System.Windows.Forms.ToolStripStatusLabel cntStatusLine;
        private System.Windows.Forms.Button cntPause;
        private System.Windows.Forms.Button cntExit;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}