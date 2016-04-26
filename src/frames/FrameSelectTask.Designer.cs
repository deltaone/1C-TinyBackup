namespace Core
{
    partial class FrameSelectTask
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
            this.cntTasks = new System.Windows.Forms.CheckedListBox();
            this.cntOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cntTasks
            // 
            this.cntTasks.CheckOnClick = true;
            this.cntTasks.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cntTasks.FormattingEnabled = true;
            this.cntTasks.Location = new System.Drawing.Point(2, 8);
            this.cntTasks.Name = "cntTasks";
            this.cntTasks.Size = new System.Drawing.Size(119, 123);
            this.cntTasks.TabIndex = 0;
            // 
            // cntOK
            // 
            this.cntOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cntOK.Location = new System.Drawing.Point(2, 137);
            this.cntOK.Name = "cntOK";
            this.cntOK.Size = new System.Drawing.Size(119, 28);
            this.cntOK.TabIndex = 2;
            this.cntOK.Text = "BACKUP";
            this.cntOK.UseVisualStyleBackColor = true;
            this.cntOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // FrameSelectTask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(125, 171);
            this.Controls.Add(this.cntOK);
            this.Controls.Add(this.cntTasks);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FrameSelectTask";
            this.Text = "Select tasks ...";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox cntTasks;
        private System.Windows.Forms.Button cntOK;
    }
}