using HueWrapper.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HueWrapper
{
    class HueWrapperContext : ApplicationContext
    {
        private HueManager manager;

        private HueWrapper form;

        private NotifyIcon TrayIcon;
        private ContextMenuStrip TrayIconContextMenu;
        private ToolStripMenuItem FormToggleItem;
        private ToolStripMenuItem CloseMenuItem;

        public HueWrapperContext()
        {
            manager = new HueManager();

            if (manager.Setup())
            {
                form = new HueWrapper(manager);
            }
            else
            {
                MessageBox.Show("Setup has failed!\nApplication will close now.", "Setup has failed!", MessageBoxButtons.OK);
                Application.Exit();
                return;
            }

            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);

            InitializeComponent();
            TrayIcon.Visible = true;
        }

        private void InitializeComponent()
        {
            TrayIcon = new NotifyIcon();
            TrayIcon.Text = "HueWrapper";

            TrayIcon.Icon = Properties.Resources.HueWrapper;

            TrayIcon.DoubleClick += TrayIcon_DoubleClick;

            TrayIconContextMenu = new ContextMenuStrip();
            FormToggleItem = new ToolStripMenuItem();
            CloseMenuItem = new ToolStripMenuItem();
            TrayIconContextMenu.SuspendLayout();

            // 
            // TrayIconContextMenu
            // 
            this.TrayIconContextMenu.Items.AddRange(new ToolStripItem[] {
                this.FormToggleItem,
                this.CloseMenuItem,
            });
            this.TrayIconContextMenu.Name = "HueWrapper";
            this.TrayIconContextMenu.Size = new Size(153, 70);

            //
            // FormMenuItem
            //
            this.FormToggleItem.Name = "FormToggle";
            this.FormToggleItem.Size = new Size(152, 152);
            this.FormToggleItem.Text = form.Visible ? "Hide" : "Show";
            this.FormToggleItem.Click += new EventHandler(this.FormToggleButton_Click);

            // 
            // CloseMenuItem
            // 
            this.CloseMenuItem.Name = "CloseMenuItem";
            this.CloseMenuItem.Size = new Size(152, 22);
            this.CloseMenuItem.Text = "Close HueWrapper";
            this.CloseMenuItem.Click += new EventHandler(this.CloseMenuItem_Click);

            TrayIconContextMenu.ResumeLayout(false);
            TrayIcon.ContextMenuStrip = TrayIconContextMenu;
        }

        private void FormToggleButton_Click(object sender, EventArgs e)
        {
            ToggleForm();
        }
        private void ToggleForm()
        {
            if (form != null && !form.IsDisposed && form.Visible)
            {
                form.ToggleForm(false);
                this.FormToggleItem.Text = "Show";
            }
            else
            {
                if (form == null || form.IsDisposed)
                {
                    form = null;
                    form = new HueWrapper(manager);
                }
                form.ToggleForm(true);
                this.FormToggleItem.Text = "Hide";
            }
        }

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close HueWrapper?",
                    "Close HueWrapper?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            form.Close();
            form.Dispose();
            form = null;

            TrayIcon.Visible = false;
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            ToggleForm();
        }
    }
}
