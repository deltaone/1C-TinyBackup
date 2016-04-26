using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

using Core;

namespace ControlsEx
{
    public class RichTextLabel : RichTextBox
    {   // http://stackoverflow.com/questions/582312/how-to-hide-the-caret-in-a-richtextbox
        public RichTextLabel()
        {
            base.ReadOnly = true;
            base.BorderStyle = BorderStyle.None;
            base.TabStop = false;
            base.SetStyle(ControlStyles.Selectable, false);
            base.SetStyle(ControlStyles.UserMouse, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            base.Font = new Font("Consolas", 10, FontStyle.Regular);
            base.ForeColor = System.Drawing.Color.Blue;

            base.MouseEnter += delegate(object sender, EventArgs e)
            {
                this.Cursor = Cursors.Default;
            };
        }
        
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x204) return; // WM_RBUTTONDOWN
            if (m.Msg == 0x205) return; // WM_RBUTTONUP
            base.WndProc(ref m);
        }

        public void AppendColoredText(string text, Color color)
        {
            SelectionStart = TextLength;
            SelectionLength = 0;
            SelectionColor = color;
            AppendText(text);
            SelectionColor = ForeColor;
        }
    }
}
