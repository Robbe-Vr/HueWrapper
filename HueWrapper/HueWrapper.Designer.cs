
namespace HueWrapper
{
    partial class HueWrapper
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.HueCommandSender = new System.Windows.Forms.Button();
            this.CommandInputTextBox = new System.Windows.Forms.TextBox();
            this.CommandResultLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // HueCommandSender
            // 
            this.HueCommandSender.Location = new System.Drawing.Point(64, 154);
            this.HueCommandSender.Name = "HueCommandSender";
            this.HueCommandSender.Size = new System.Drawing.Size(75, 23);
            this.HueCommandSender.TabIndex = 0;
            this.HueCommandSender.Text = "Send Command";
            this.HueCommandSender.UseVisualStyleBackColor = true;
            this.HueCommandSender.Click += new System.EventHandler(this.HueCommandSender_Click);
            // 
            // CommandInputTextBox
            // 
            this.CommandInputTextBox.Location = new System.Drawing.Point(64, 116);
            this.CommandInputTextBox.Name = "CommandInputTextBox";
            this.CommandInputTextBox.Size = new System.Drawing.Size(416, 23);
            this.CommandInputTextBox.TabIndex = 1;
            // 
            // CommandResultLabel
            // 
            this.CommandResultLabel.AutoSize = true;
            this.CommandResultLabel.Location = new System.Drawing.Point(64, 201);
            this.CommandResultLabel.Name = "CommandResultLabel";
            this.CommandResultLabel.Size = new System.Drawing.Size(154, 15);
            this.CommandResultLabel.TabIndex = 2;
            this.CommandResultLabel.Text = "No command executed yet.";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.CommandResultLabel);
            this.Controls.Add(this.CommandInputTextBox);
            this.Controls.Add(this.HueCommandSender);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button HueCommandSender;
        private System.Windows.Forms.TextBox CommandInputTextBox;
        private System.Windows.Forms.Label CommandResultLabel;
    }
}

