using HueWrapper.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HueWrapper
{
    public partial class HueWrapper : Form
    {
        private HueManager manager;

        public HueWrapper(HueManager manager)
        {
            this.manager = manager;

            InitializeComponent();
        }

        public void ToggleForm(bool show)
        {
            if (show)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => { this.Show(); }));
                }
                else Show();
            }
            else
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => { this.Hide(); }));
                }
                else Hide();
            }
        }

        private void HueCommandSender_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(CommandInputTextBox.Text))
            {
                string response = manager.ProcessMessage(this.CommandInputTextBox.Text);

                this.CommandResultLabel.Text = response;
            }
        }
    }
}
