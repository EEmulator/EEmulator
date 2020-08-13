namespace EEmulatorLauncher
{
    partial class WorldSelectionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorldSelectionDialog));
            this.lbSelectVersion = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pbMinimap = new System.Windows.Forms.PictureBox();
            this.btnPlayNow = new System.Windows.Forms.Button();
            this.comboSelectedVersion = new DarkUI.Controls.DarkComboBox();
            this.gbDetails = new DarkUI.Controls.DarkGroupBox();
            this.txtDetails = new DarkUI.Controls.DarkTextBox();
            this.comboWorlds = new DarkUI.Controls.DarkComboBox();
            this.btnLoad = new DarkUI.Controls.DarkButton();
            this.txtUsername = new DarkUI.Controls.DarkTextBox();
            this.dockPanel = new DarkUI.Docking.DarkDockPanel();
            this.btnCopy = new DarkUI.Controls.DarkButton();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbMinimap)).BeginInit();
            this.gbDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbSelectVersion
            // 
            this.lbSelectVersion.AutoSize = true;
            this.lbSelectVersion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.lbSelectVersion.ForeColor = System.Drawing.Color.White;
            this.lbSelectVersion.Location = new System.Drawing.Point(199, 395);
            this.lbSelectVersion.Name = "lbSelectVersion";
            this.lbSelectVersion.Size = new System.Drawing.Size(45, 13);
            this.lbSelectVersion.TabIndex = 11;
            this.lbSelectVersion.Text = "Version:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(5, 427);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(508, 26);
            this.label1.TabIndex = 12;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // pbMinimap
            // 
            this.pbMinimap.BackColor = System.Drawing.Color.Black;
            this.pbMinimap.Location = new System.Drawing.Point(515, 38);
            this.pbMinimap.Name = "pbMinimap";
            this.pbMinimap.Size = new System.Drawing.Size(400, 200);
            this.pbMinimap.TabIndex = 6;
            this.pbMinimap.TabStop = false;
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
            this.btnPlayNow.Location = new System.Drawing.Point(422, 389);
            this.btnPlayNow.Margin = new System.Windows.Forms.Padding(0);
            this.btnPlayNow.Name = "btnPlayNow";
            this.btnPlayNow.Size = new System.Drawing.Size(84, 30);
            this.btnPlayNow.TabIndex = 5;
            this.btnPlayNow.UseVisualStyleBackColor = false;
            this.btnPlayNow.Click += new System.EventHandler(this.btnPlayNow_Click);
            // 
            // comboSelectedVersion
            // 
            this.comboSelectedVersion.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboSelectedVersion.FormattingEnabled = true;
            this.comboSelectedVersion.Items.AddRange(new object[] {
            "0.5.0.0",
            "0.7.0.0",
            "0.8.0.0",
            "Everybody Edits v89",
            "Everybody Edits v188",
            "Everybody Edits v225"});
            this.comboSelectedVersion.Location = new System.Drawing.Point(250, 392);
            this.comboSelectedVersion.Name = "comboSelectedVersion";
            this.comboSelectedVersion.Size = new System.Drawing.Size(158, 21);
            this.comboSelectedVersion.TabIndex = 10;
            // 
            // gbDetails
            // 
            this.gbDetails.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.gbDetails.Controls.Add(this.txtDetails);
            this.gbDetails.Location = new System.Drawing.Point(12, 65);
            this.gbDetails.Name = "gbDetails";
            this.gbDetails.Size = new System.Drawing.Size(497, 321);
            this.gbDetails.TabIndex = 7;
            this.gbDetails.TabStop = false;
            this.gbDetails.Text = "TSON Representation:";
            // 
            // txtDetails
            // 
            this.txtDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDetails.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtDetails.Location = new System.Drawing.Point(3, 16);
            this.txtDetails.Multiline = true;
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.ReadOnly = true;
            this.txtDetails.Size = new System.Drawing.Size(491, 302);
            this.txtDetails.TabIndex = 10;
            // 
            // comboWorlds
            // 
            this.comboWorlds.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboWorlds.FormattingEnabled = true;
            this.comboWorlds.Location = new System.Drawing.Point(12, 38);
            this.comboWorlds.Name = "comboWorlds";
            this.comboWorlds.Size = new System.Drawing.Size(411, 21);
            this.comboWorlds.TabIndex = 3;
            this.comboWorlds.SelectedIndexChanged += new System.EventHandler(this.comboWorlds_SelectedIndexChanged);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(434, 12);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Padding = new System.Windows.Forms.Padding(5);
            this.btnLoad.Size = new System.Drawing.Size(75, 20);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "Load";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // txtUsername
            // 
            this.txtUsername.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsername.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtUsername.Location = new System.Drawing.Point(235, 12);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(188, 20);
            this.txtUsername.TabIndex = 1;
            this.txtUsername.Text = "username";
            this.txtUsername.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // dockPanel
            // 
            this.dockPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.Location = new System.Drawing.Point(0, 0);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Size = new System.Drawing.Size(921, 463);
            this.dockPanel.TabIndex = 0;
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(434, 38);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Padding = new System.Windows.Forms.Padding(5);
            this.btnCopy.Size = new System.Drawing.Size(75, 21);
            this.btnCopy.TabIndex = 13;
            this.btnCopy.Text = "Copy";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(672, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 25);
            this.label2.TabIndex = 14;
            this.label2.Text = "Minimap";
            // 
            // WorldSelectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 463);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbSelectVersion);
            this.Controls.Add(this.comboSelectedVersion);
            this.Controls.Add(this.gbDetails);
            this.Controls.Add(this.pbMinimap);
            this.Controls.Add(this.btnPlayNow);
            this.Controls.Add(this.comboWorlds);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.dockPanel);
            this.Name = "WorldSelectionDialog";
            this.ShowIcon = false;
            this.Text = "Select a World";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pbMinimap)).EndInit();
            this.gbDetails.ResumeLayout(false);
            this.gbDetails.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DarkUI.Docking.DarkDockPanel dockPanel;
        private DarkUI.Controls.DarkTextBox txtUsername;
        private DarkUI.Controls.DarkButton btnLoad;
        private DarkUI.Controls.DarkComboBox comboWorlds;
        private System.Windows.Forms.Button btnPlayNow;
        private System.Windows.Forms.PictureBox pbMinimap;
        private DarkUI.Controls.DarkGroupBox gbDetails;
        private DarkUI.Controls.DarkTextBox txtDetails;
        private DarkUI.Controls.DarkComboBox comboSelectedVersion;
        private System.Windows.Forms.Label lbSelectVersion;
        private System.Windows.Forms.Label label1;
        private DarkUI.Controls.DarkButton btnCopy;
        private System.Windows.Forms.Label label2;
    }
}