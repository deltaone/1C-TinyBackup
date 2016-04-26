using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Core
{
    public partial class FrameSelectTask : Form
    {
        public FrameSelectTask()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;

            var items = cntTasks.Items;
            foreach (var r in Task.records)
            {
                items.Add(Task.records[r.Key]["id"], (Task.records[r.Key]["enabled"] == "1" ?  true : false));
            }            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cntTasks.Items.Count; i++)
            {
                var id = cntTasks.Items[i] as string;
                if (cntTasks.GetItemChecked(i)) Task.records[id]["enabled"] = "1";
                else Task.records[id]["enabled"] = "0";
                GM.Log("SELECT-TASK", "Task state [" + id + "] = '" + Task.records[id]["enabled"] + "'");
            }                
            Close();
        }
    }
}
