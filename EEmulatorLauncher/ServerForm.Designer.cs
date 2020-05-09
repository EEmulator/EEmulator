namespace EEmulatorLauncher
{
    partial class ServerForm
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
            this.refreshLabel = new System.Windows.Forms.LinkLabel();
            this.lbServerName = new System.Windows.Forms.Label();
            this.lbServerLocation = new System.Windows.Forms.Label();
            this.selectionPanel = new DarkUI.Controls.DarkSectionPanel();
            this.lbServerSelect = new DarkUI.Controls.DarkGroupBox();
            this.ddServers = new DarkUI.Controls.DarkDropdownList();
            this.btnAddServer = new System.Windows.Forms.Button();
            this.btnPlayNow = new System.Windows.Forms.Button();
            this.lbDescription = new DarkUI.Controls.DarkGroupBox();
            this.textDescription = new DarkUI.Controls.DarkTextBox();
            this.selectionPanel.SuspendLayout();
            this.lbServerSelect.SuspendLayout();
            this.lbDescription.SuspendLayout();
            this.SuspendLayout();
            // 
            // refreshLabel
            // 
            this.refreshLabel.AutoSize = true;
            this.refreshLabel.BackColor = System.Drawing.Color.Transparent;
            this.refreshLabel.LinkColor = System.Drawing.Color.White;
            this.refreshLabel.Location = new System.Drawing.Point(348, 6);
            this.refreshLabel.Name = "refreshLabel";
            this.refreshLabel.Size = new System.Drawing.Size(44, 13);
            this.refreshLabel.TabIndex = 2;
            this.refreshLabel.TabStop = true;
            this.refreshLabel.Text = "Refresh";
            this.refreshLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.refreshLabel_LinkClicked);
            // 
            // lbServerName
            // 
            this.lbServerName.AutoSize = true;
            this.lbServerName.BackColor = System.Drawing.Color.Transparent;
            this.lbServerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbServerName.ForeColor = System.Drawing.Color.White;
            this.lbServerName.Location = new System.Drawing.Point(12, 130);
            this.lbServerName.Name = "lbServerName";
            this.lbServerName.Size = new System.Drawing.Size(84, 16);
            this.lbServerName.TabIndex = 5;
            this.lbServerName.Text = "Local Server";
            // 
            // lbServerLocation
            // 
            this.lbServerLocation.AutoSize = true;
            this.lbServerLocation.BackColor = System.Drawing.Color.Transparent;
            this.lbServerLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbServerLocation.ForeColor = System.Drawing.Color.DarkGray;
            this.lbServerLocation.Location = new System.Drawing.Point(12, 146);
            this.lbServerLocation.Name = "lbServerLocation";
            this.lbServerLocation.Size = new System.Drawing.Size(62, 16);
            this.lbServerLocation.TabIndex = 6;
            this.lbServerLocation.Text = "localhost";
            // 
            // selectionPanel
            // 
            this.selectionPanel.Controls.Add(this.lbServerSelect);
            this.selectionPanel.Controls.Add(this.lbDescription);
            this.selectionPanel.Controls.Add(this.refreshLabel);
            this.selectionPanel.Location = new System.Drawing.Point(1, 0);
            this.selectionPanel.Name = "selectionPanel";
            this.selectionPanel.SectionHeader = "  Servers";
            this.selectionPanel.Size = new System.Drawing.Size(398, 127);
            this.selectionPanel.TabIndex = 7;
            // 
            // lbServerSelect
            // 
            this.lbServerSelect.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lbServerSelect.Controls.Add(this.ddServers);
            this.lbServerSelect.Controls.Add(this.btnAddServer);
            this.lbServerSelect.Controls.Add(this.btnPlayNow);
            this.lbServerSelect.Location = new System.Drawing.Point(6, 28);
            this.lbServerSelect.Name = "lbServerSelect";
            this.lbServerSelect.Size = new System.Drawing.Size(189, 95);
            this.lbServerSelect.TabIndex = 5;
            this.lbServerSelect.TabStop = false;
            this.lbServerSelect.Text = "Select a server to join.";
            // 
            // ddServers
            // 
            this.ddServers.Location = new System.Drawing.Point(8, 16);
            this.ddServers.Name = "ddServers";
            this.ddServers.Size = new System.Drawing.Size(175, 26);
            this.ddServers.TabIndex = 0;
            // 
            // btnAddServer
            // 
            this.btnAddServer.BackColor = System.Drawing.Color.Transparent;
            this.btnAddServer.BackgroundImage = global::EEmulatorLauncher.Properties.Resources.addServerButton;
            this.btnAddServer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnAddServer.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnAddServer.FlatAppearance.BorderSize = 0;
            this.btnAddServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddServer.Font = new System.Drawing.Font("Georgia", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddServer.ForeColor = System.Drawing.Color.Transparent;
            this.btnAddServer.Location = new System.Drawing.Point(8, 52);
            this.btnAddServer.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddServer.Name = "btnAddServer";
            this.btnAddServer.Size = new System.Drawing.Size(84, 30);
            this.btnAddServer.TabIndex = 3;
            this.btnAddServer.UseVisualStyleBackColor = false;
            this.btnAddServer.Click += new System.EventHandler(this.btnAddServer_Click);
            // 
            // btnPlayNow
            // 
            this.btnPlayNow.BackColor = System.Drawing.Color.Transparent;
            this.btnPlayNow.BackgroundImage = global::EEmulatorLauncher.Properties.Resources.playNowButton;
            this.btnPlayNow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnPlayNow.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnPlayNow.FlatAppearance.BorderSize = 0;
            this.btnPlayNow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlayNow.Font = new System.Drawing.Font("Georgia", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlayNow.ForeColor = System.Drawing.Color.Transparent;
            this.btnPlayNow.Location = new System.Drawing.Point(98, 52);
            this.btnPlayNow.Margin = new System.Windows.Forms.Padding(0);
            this.btnPlayNow.Name = "btnPlayNow";
            this.btnPlayNow.Size = new System.Drawing.Size(84, 30);
            this.btnPlayNow.TabIndex = 4;
            this.btnPlayNow.UseVisualStyleBackColor = false;
            this.btnPlayNow.Click += new System.EventHandler(this.btnPlayNow_Click);
            // 
            // lbDescription
            // 
            this.lbDescription.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lbDescription.Controls.Add(this.textDescription);
            this.lbDescription.Location = new System.Drawing.Point(198, 28);
            this.lbDescription.Name = "lbDescription";
            this.lbDescription.Size = new System.Drawing.Size(189, 95);
            this.lbDescription.TabIndex = 4;
            this.lbDescription.TabStop = false;
            this.lbDescription.Text = "Description";
            // 
            // textDescription
            // 
            this.textDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.textDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.textDescription.Location = new System.Drawing.Point(3, 16);
            this.textDescription.Multiline = true;
            this.textDescription.Name = "textDescription";
            this.textDescription.ReadOnly = true;
            this.textDescription.Size = new System.Drawing.Size(183, 76);
            this.textDescription.TabIndex = 0;
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(400, 172);
            this.Controls.Add(this.selectionPanel);
            this.Controls.Add(this.lbServerLocation);
            this.Controls.Add(this.lbServerName);
            this.MaximumSize = new System.Drawing.Size(416, 413);
            this.MinimumSize = new System.Drawing.Size(416, 211);
            this.Name = "ServerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Everybody Edits Network";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServerForm_FormClosing);
            this.Load += new System.EventHandler(this.ServerForm_Load);
            this.selectionPanel.ResumeLayout(false);
            this.selectionPanel.PerformLayout();
            this.lbServerSelect.ResumeLayout(false);
            this.lbDescription.ResumeLayout(false);
            this.lbDescription.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.LinkLabel refreshLabel;
        private System.Windows.Forms.Button btnAddServer;
        private System.Windows.Forms.Button btnPlayNow;
        private System.Windows.Forms.Label lbServerName;
        private System.Windows.Forms.Label lbServerLocation;
        private DarkUI.Controls.DarkSectionPanel selectionPanel;
        private DarkUI.Controls.DarkGroupBox lbServerSelect;
        private DarkUI.Controls.DarkDropdownList ddServers;
        private DarkUI.Controls.DarkGroupBox lbDescription;
        private DarkUI.Controls.DarkTextBox textDescription;
    }
}