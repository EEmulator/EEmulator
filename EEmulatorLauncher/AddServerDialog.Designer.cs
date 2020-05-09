namespace EEmulatorLauncher
{
    partial class AddServerDialog
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
            this.sectionPanel = new DarkUI.Controls.DarkSectionPanel();
            this.btnOk = new DarkUI.Controls.DarkButton();
            this.btnCancel = new DarkUI.Controls.DarkButton();
            this.inputPort = new DarkUI.Controls.DarkNumericUpDown();
            this.inputAddress = new DarkUI.Controls.DarkTextBox();
            this.sectionPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inputPort)).BeginInit();
            this.SuspendLayout();
            // 
            // sectionPanel
            // 
            this.sectionPanel.Controls.Add(this.btnOk);
            this.sectionPanel.Controls.Add(this.btnCancel);
            this.sectionPanel.Controls.Add(this.inputPort);
            this.sectionPanel.Controls.Add(this.inputAddress);
            this.sectionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sectionPanel.Location = new System.Drawing.Point(0, 0);
            this.sectionPanel.Name = "sectionPanel";
            this.sectionPanel.SectionHeader = "               Address                          Port";
            this.sectionPanel.Size = new System.Drawing.Size(243, 96);
            this.sectionPanel.TabIndex = 0;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(159, 63);
            this.btnOk.Name = "btnOk";
            this.btnOk.Padding = new System.Windows.Forms.Padding(5);
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(78, 63);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(5);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // inputPort
            // 
            this.inputPort.Location = new System.Drawing.Point(162, 28);
            this.inputPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.inputPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.inputPort.Name = "inputPort";
            this.inputPort.Size = new System.Drawing.Size(72, 20);
            this.inputPort.TabIndex = 1;
            this.inputPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.inputPort.Value = new decimal(new int[] {
            8184,
            0,
            0,
            0});
            // 
            // inputAddress
            // 
            this.inputAddress.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.inputAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inputAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.inputAddress.Location = new System.Drawing.Point(12, 28);
            this.inputAddress.Name = "inputAddress";
            this.inputAddress.Size = new System.Drawing.Size(141, 20);
            this.inputAddress.TabIndex = 0;
            this.inputAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // AddServerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(243, 96);
            this.Controls.Add(this.sectionPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximumSize = new System.Drawing.Size(259, 135);
            this.MinimumSize = new System.Drawing.Size(259, 135);
            this.Name = "AddServerDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add s server";
            this.TopMost = true;
            this.sectionPanel.ResumeLayout(false);
            this.sectionPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inputPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DarkUI.Controls.DarkSectionPanel sectionPanel;
        private DarkUI.Controls.DarkButton btnOk;
        private DarkUI.Controls.DarkButton btnCancel;
        private DarkUI.Controls.DarkNumericUpDown inputPort;
        private DarkUI.Controls.DarkTextBox inputAddress;
    }
}