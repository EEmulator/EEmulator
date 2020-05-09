namespace EEmulatorLauncher
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.topPanel = new System.Windows.Forms.Panel();
            this.btnNetwork = new System.Windows.Forms.Button();
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPlay0500 = new System.Windows.Forms.Button();
            this.secretLabel = new System.Windows.Forms.Label();
            this.btnPlay0700 = new System.Windows.Forms.Button();
            this.secretLabel2 = new System.Windows.Forms.Label();
            this.btnPlay0800 = new System.Windows.Forms.Button();
            this.secretLabel3 = new System.Windows.Forms.Label();
            this.btnPlay89 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPlay188 = new System.Windows.Forms.Button();
            this.birb = new System.Windows.Forms.PictureBox();
            this.topPanel.SuspendLayout();
            this.flowLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.birb)).BeginInit();
            this.SuspendLayout();
            // 
            // bottomPanel
            // 
            this.bottomPanel.BackgroundImage = global::EEmulatorLauncher.Properties.Resources.bg_89;
            this.bottomPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomPanel.Location = new System.Drawing.Point(0, 40);
            this.bottomPanel.Margin = new System.Windows.Forms.Padding(0);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(639, 481);
            this.bottomPanel.TabIndex = 1;
            // 
            // topPanel
            // 
            this.topPanel.BackColor = System.Drawing.Color.Transparent;
            this.topPanel.BackgroundImage = global::EEmulatorLauncher.Properties.Resources._113;
            this.topPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.topPanel.Controls.Add(this.btnNetwork);
            this.topPanel.Controls.Add(this.flowLayoutPanel);
            this.topPanel.Controls.Add(this.birb);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Margin = new System.Windows.Forms.Padding(0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(639, 40);
            this.topPanel.TabIndex = 0;
            // 
            // btnNetwork
            // 
            this.btnNetwork.BackgroundImage = global::EEmulatorLauncher.Properties.Resources.networkButton;
            this.btnNetwork.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnNetwork.FlatAppearance.BorderSize = 0;
            this.btnNetwork.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNetwork.ForeColor = System.Drawing.Color.Black;
            this.btnNetwork.Location = new System.Drawing.Point(521, 7);
            this.btnNetwork.Margin = new System.Windows.Forms.Padding(0);
            this.btnNetwork.Name = "btnNetwork";
            this.btnNetwork.Size = new System.Drawing.Size(84, 30);
            this.btnNetwork.TabIndex = 10;
            this.btnNetwork.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnNetwork.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnNetwork.UseVisualStyleBackColor = true;
            this.btnNetwork.Click += new System.EventHandler(this.btnNetwork_Click);
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.Controls.Add(this.btnPlay0500);
            this.flowLayoutPanel.Controls.Add(this.secretLabel);
            this.flowLayoutPanel.Controls.Add(this.btnPlay0700);
            this.flowLayoutPanel.Controls.Add(this.secretLabel2);
            this.flowLayoutPanel.Controls.Add(this.btnPlay0800);
            this.flowLayoutPanel.Controls.Add(this.secretLabel3);
            this.flowLayoutPanel.Controls.Add(this.btnPlay89);
            this.flowLayoutPanel.Controls.Add(this.label1);
            this.flowLayoutPanel.Controls.Add(this.btnPlay188);
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Padding = new System.Windows.Forms.Padding(8, 7, 0, 0);
            this.flowLayoutPanel.Size = new System.Drawing.Size(511, 40);
            this.flowLayoutPanel.TabIndex = 2;
            // 
            // btnPlay0500
            // 
            this.btnPlay0500.BackgroundImage = global::EEmulatorLauncher.Properties.Resources.play0500;
            this.btnPlay0500.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnPlay0500.FlatAppearance.BorderSize = 0;
            this.btnPlay0500.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay0500.ForeColor = System.Drawing.Color.Black;
            this.btnPlay0500.Location = new System.Drawing.Point(8, 7);
            this.btnPlay0500.Margin = new System.Windows.Forms.Padding(0);
            this.btnPlay0500.Name = "btnPlay0500";
            this.btnPlay0500.Size = new System.Drawing.Size(56, 30);
            this.btnPlay0500.TabIndex = 1;
            this.btnPlay0500.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnPlay0500.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPlay0500.UseVisualStyleBackColor = true;
            this.btnPlay0500.Click += new System.EventHandler(this.btnPlay0500_Click);
            this.btnPlay0500.MouseHover += new System.EventHandler(this.btnPlay0500_MouseHover);
            // 
            // secretLabel
            // 
            this.secretLabel.AutoSize = true;
            this.secretLabel.Location = new System.Drawing.Point(67, 7);
            this.secretLabel.Name = "secretLabel";
            this.secretLabel.Size = new System.Drawing.Size(0, 13);
            this.secretLabel.TabIndex = 2;
            // 
            // btnPlay0700
            // 
            this.btnPlay0700.BackgroundImage = global::EEmulatorLauncher.Properties.Resources.play0700;
            this.btnPlay0700.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnPlay0700.FlatAppearance.BorderSize = 0;
            this.btnPlay0700.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay0700.ForeColor = System.Drawing.Color.Black;
            this.btnPlay0700.Location = new System.Drawing.Point(70, 7);
            this.btnPlay0700.Margin = new System.Windows.Forms.Padding(0);
            this.btnPlay0700.Name = "btnPlay0700";
            this.btnPlay0700.Size = new System.Drawing.Size(56, 30);
            this.btnPlay0700.TabIndex = 3;
            this.btnPlay0700.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnPlay0700.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPlay0700.UseVisualStyleBackColor = true;
            this.btnPlay0700.Click += new System.EventHandler(this.btnPlay0700_Click);
            this.btnPlay0700.MouseHover += new System.EventHandler(this.btnPlay0700_MouseHover);
            // 
            // secretLabel2
            // 
            this.secretLabel2.AutoSize = true;
            this.secretLabel2.Location = new System.Drawing.Point(129, 7);
            this.secretLabel2.Name = "secretLabel2";
            this.secretLabel2.Size = new System.Drawing.Size(0, 13);
            this.secretLabel2.TabIndex = 4;
            // 
            // btnPlay0800
            // 
            this.btnPlay0800.BackgroundImage = global::EEmulatorLauncher.Properties.Resources.play0080;
            this.btnPlay0800.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnPlay0800.FlatAppearance.BorderSize = 0;
            this.btnPlay0800.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay0800.ForeColor = System.Drawing.Color.Black;
            this.btnPlay0800.Location = new System.Drawing.Point(132, 7);
            this.btnPlay0800.Margin = new System.Windows.Forms.Padding(0);
            this.btnPlay0800.Name = "btnPlay0800";
            this.btnPlay0800.Size = new System.Drawing.Size(56, 30);
            this.btnPlay0800.TabIndex = 5;
            this.btnPlay0800.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnPlay0800.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPlay0800.UseVisualStyleBackColor = true;
            this.btnPlay0800.Click += new System.EventHandler(this.btnPlay0800_Click);
            this.btnPlay0800.MouseHover += new System.EventHandler(this.btnPlay0800_MouseHover);
            // 
            // secretLabel3
            // 
            this.secretLabel3.AutoSize = true;
            this.secretLabel3.Location = new System.Drawing.Point(191, 7);
            this.secretLabel3.Name = "secretLabel3";
            this.secretLabel3.Size = new System.Drawing.Size(0, 13);
            this.secretLabel3.TabIndex = 6;
            // 
            // btnPlay89
            // 
            this.btnPlay89.BackgroundImage = global::EEmulatorLauncher.Properties.Resources.playv89;
            this.btnPlay89.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnPlay89.FlatAppearance.BorderSize = 0;
            this.btnPlay89.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay89.ForeColor = System.Drawing.Color.Black;
            this.btnPlay89.Location = new System.Drawing.Point(194, 7);
            this.btnPlay89.Margin = new System.Windows.Forms.Padding(0);
            this.btnPlay89.Name = "btnPlay89";
            this.btnPlay89.Size = new System.Drawing.Size(56, 30);
            this.btnPlay89.TabIndex = 7;
            this.btnPlay89.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnPlay89.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPlay89.UseVisualStyleBackColor = true;
            this.btnPlay89.Click += new System.EventHandler(this.btnPlay89_Click);
            this.btnPlay89.MouseHover += new System.EventHandler(this.btnPlay89_MouseHover);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(253, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 8;
            // 
            // btnPlay188
            // 
            this.btnPlay188.BackgroundImage = global::EEmulatorLauncher.Properties.Resources.playv188;
            this.btnPlay188.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnPlay188.FlatAppearance.BorderSize = 0;
            this.btnPlay188.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay188.ForeColor = System.Drawing.Color.Black;
            this.btnPlay188.Location = new System.Drawing.Point(256, 7);
            this.btnPlay188.Margin = new System.Windows.Forms.Padding(0);
            this.btnPlay188.Name = "btnPlay188";
            this.btnPlay188.Size = new System.Drawing.Size(56, 30);
            this.btnPlay188.TabIndex = 9;
            this.btnPlay188.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.btnPlay188.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPlay188.UseVisualStyleBackColor = true;
            this.btnPlay188.Click += new System.EventHandler(this.btnPlay188_Click);
            this.btnPlay188.MouseHover += new System.EventHandler(this.btnPlay188_MouseHover);
            // 
            // birb
            // 
            this.birb.BackgroundImage = global::EEmulatorLauncher.Properties.Resources._196_LobbyState_twitter_LobbyState_twitter;
            this.birb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.birb.Dock = System.Windows.Forms.DockStyle.Right;
            this.birb.Location = new System.Drawing.Point(608, 0);
            this.birb.Name = "birb";
            this.birb.Size = new System.Drawing.Size(31, 40);
            this.birb.TabIndex = 0;
            this.birb.TabStop = false;
            this.birb.Click += new System.EventHandler(this.OnBirbClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(639, 521);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.topPanel);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(655, 560);
            this.MinimumSize = new System.Drawing.Size(655, 560);
            this.Name = "MainForm";
            this.Text = "Everybody Edits Emulator";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.topPanel.ResumeLayout(false);
            this.flowLayoutPanel.ResumeLayout(false);
            this.flowLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.birb)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.PictureBox birb;
        private System.Windows.Forms.Button btnPlay0500;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.Label secretLabel;
        private System.Windows.Forms.Button btnPlay0700;
        private System.Windows.Forms.Label secretLabel2;
        private System.Windows.Forms.Button btnPlay0800;
        private System.Windows.Forms.Label secretLabel3;
        private System.Windows.Forms.Button btnPlay89;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPlay188;
        private System.Windows.Forms.Button btnNetwork;
    }
}

