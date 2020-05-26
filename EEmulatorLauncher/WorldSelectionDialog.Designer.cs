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
            this.dockPanel = new DarkUI.Docking.DarkDockPanel();
            this.txtUsername = new DarkUI.Controls.DarkTextBox();
            this.btnLoad = new DarkUI.Controls.DarkButton();
            this.comboWorlds = new DarkUI.Controls.DarkComboBox();
            this.gbDetails = new DarkUI.Controls.DarkGroupBox();
            this.txtDetails = new DarkUI.Controls.DarkTextBox();
            this.pbMinimap = new System.Windows.Forms.PictureBox();
            this.btnPlayNow = new System.Windows.Forms.Button();
            this.comboSelectedVersion = new DarkUI.Controls.DarkComboBox();
            this.lbSelectVersion = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.gbDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbMinimap)).BeginInit();
            this.SuspendLayout();
            // 
            // dockPanel
            // 
            this.dockPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.Location = new System.Drawing.Point(0, 0);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Size = new System.Drawing.Size(765, 370);
            this.dockPanel.TabIndex = 0;
            // 
            // txtUsername
            // 
            this.txtUsername.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsername.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtUsername.Location = new System.Drawing.Point(12, 12);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(229, 20);
            this.txtUsername.TabIndex = 1;
            this.txtUsername.Text = "username";
            this.txtUsername.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(247, 12);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Padding = new System.Windows.Forms.Padding(5);
            this.btnLoad.Size = new System.Drawing.Size(75, 20);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "Load";
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // comboWorlds
            // 
            this.comboWorlds.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboWorlds.FormattingEnabled = true;
            this.comboWorlds.Location = new System.Drawing.Point(12, 38);
            this.comboWorlds.Name = "comboWorlds";
            this.comboWorlds.Size = new System.Drawing.Size(310, 21);
            this.comboWorlds.TabIndex = 3;
            this.comboWorlds.SelectedIndexChanged += new System.EventHandler(this.comboWorlds_SelectedIndexChanged);
            // 
            // gbDetails
            // 
            this.gbDetails.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.gbDetails.Controls.Add(this.txtDetails);
            this.gbDetails.Location = new System.Drawing.Point(12, 65);
            this.gbDetails.Name = "gbDetails";
            this.gbDetails.Size = new System.Drawing.Size(310, 191);
            this.gbDetails.TabIndex = 7;
            this.gbDetails.TabStop = false;
            this.gbDetails.Text = "TSON Details:";
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
            this.txtDetails.Size = new System.Drawing.Size(304, 172);
            this.txtDetails.TabIndex = 10;
            // 
            // pbMinimap
            // 
            this.pbMinimap.BackColor = System.Drawing.Color.Black;
            this.pbMinimap.Location = new System.Drawing.Point(348, 12);
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
            this.btnPlayNow.Location = new System.Drawing.Point(235, 264);
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
            "Everybody Edits v188"});
            this.comboSelectedVersion.Location = new System.Drawing.Point(63, 268);
            this.comboSelectedVersion.Name = "comboSelectedVersion";
            this.comboSelectedVersion.Size = new System.Drawing.Size(158, 21);
            this.comboSelectedVersion.TabIndex = 10;
            // 
            // lbSelectVersion
            // 
            this.lbSelectVersion.AutoSize = true;
            this.lbSelectVersion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.lbSelectVersion.ForeColor = System.Drawing.Color.White;
            this.lbSelectVersion.Location = new System.Drawing.Point(14, 271);
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
            this.label1.Location = new System.Drawing.Point(9, 296);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(333, 65);
            this.label1.TabIndex = 12;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // WorldSelectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 370);
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
            this.gbDetails.ResumeLayout(false);
            this.gbDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbMinimap)).EndInit();
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
    }
}