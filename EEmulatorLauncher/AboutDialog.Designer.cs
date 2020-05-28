namespace EEmulatorLauncher
{
    partial class AboutDialog
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
            this.pbConfetti = new System.Windows.Forms.PictureBox();
            this.linkSource = new System.Windows.Forms.LinkLabel();
            this.linkForums = new System.Windows.Forms.LinkLabel();
            this.pbHeart = new System.Windows.Forms.PictureBox();
            this.lbThanksEveryone = new System.Windows.Forms.Label();
            this.lbJesse = new System.Windows.Forms.Label();
            this.lbAnd = new System.Windows.Forms.Label();
            this.lbMadeBy = new System.Windows.Forms.Label();
            this.lbMiou = new System.Windows.Forms.Label();
            this.pbBird = new System.Windows.Forms.PictureBox();
            this.lbHeader = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbConfetti)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbHeart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBird)).BeginInit();
            this.SuspendLayout();
            // 
            // pbConfetti
            // 
            this.pbConfetti.BackColor = System.Drawing.Color.Transparent;
            this.pbConfetti.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbConfetti.Image = global::EEmulatorLauncher.Properties.Resources.confetti;
            this.pbConfetti.Location = new System.Drawing.Point(28, -1);
            this.pbConfetti.Name = "pbConfetti";
            this.pbConfetti.Size = new System.Drawing.Size(424, 252);
            this.pbConfetti.TabIndex = 21;
            this.pbConfetti.TabStop = false;
            this.pbConfetti.Visible = false;
            // 
            // linkSource
            // 
            this.linkSource.ActiveLinkColor = System.Drawing.Color.Gainsboro;
            this.linkSource.AutoSize = true;
            this.linkSource.BackColor = System.Drawing.Color.Transparent;
            this.linkSource.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.linkSource.Location = new System.Drawing.Point(14, 225);
            this.linkSource.Name = "linkSource";
            this.linkSource.Size = new System.Drawing.Size(213, 13);
            this.linkSource.TabIndex = 31;
            this.linkSource.TabStop = true;
            this.linkSource.Text = "View the EEmulator source code on Github.";
            this.linkSource.VisitedLinkColor = System.Drawing.Color.Silver;
            this.linkSource.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkSource_LinkClicked);
            // 
            // linkForums
            // 
            this.linkForums.ActiveLinkColor = System.Drawing.Color.Gainsboro;
            this.linkForums.AutoSize = true;
            this.linkForums.BackColor = System.Drawing.Color.Transparent;
            this.linkForums.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.linkForums.Location = new System.Drawing.Point(14, 206);
            this.linkForums.Name = "linkForums";
            this.linkForums.Size = new System.Drawing.Size(195, 13);
            this.linkForums.TabIndex = 30;
            this.linkForums.TabStop = true;
            this.linkForums.Text = "View the EEmulator topic on the forums.";
            this.linkForums.VisitedLinkColor = System.Drawing.Color.Silver;
            this.linkForums.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkForums_LinkClicked);
            // 
            // pbHeart
            // 
            this.pbHeart.BackColor = System.Drawing.Color.Transparent;
            this.pbHeart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbHeart.Image = global::EEmulatorLauncher.Properties.Resources._38;
            this.pbHeart.Location = new System.Drawing.Point(212, 152);
            this.pbHeart.Name = "pbHeart";
            this.pbHeart.Size = new System.Drawing.Size(36, 35);
            this.pbHeart.TabIndex = 29;
            this.pbHeart.TabStop = false;
            this.pbHeart.Click += new System.EventHandler(this.pbHeart_Click);
            // 
            // lbThanksEveryone
            // 
            this.lbThanksEveryone.BackColor = System.Drawing.Color.Transparent;
            this.lbThanksEveryone.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbThanksEveryone.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lbThanksEveryone.Location = new System.Drawing.Point(171, 124);
            this.lbThanksEveryone.Name = "lbThanksEveryone";
            this.lbThanksEveryone.Size = new System.Drawing.Size(140, 25);
            this.lbThanksEveryone.TabIndex = 28;
            this.lbThanksEveryone.Text = "thanks, everyone!";
            this.lbThanksEveryone.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbJesse
            // 
            this.lbJesse.BackColor = System.Drawing.Color.Transparent;
            this.lbJesse.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbJesse.ForeColor = System.Drawing.Color.PowderBlue;
            this.lbJesse.Location = new System.Drawing.Point(281, 88);
            this.lbJesse.Name = "lbJesse";
            this.lbJesse.Size = new System.Drawing.Size(53, 25);
            this.lbJesse.TabIndex = 27;
            this.lbJesse.Text = "jesse";
            this.lbJesse.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbAnd
            // 
            this.lbAnd.BackColor = System.Drawing.Color.Transparent;
            this.lbAnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAnd.ForeColor = System.Drawing.Color.White;
            this.lbAnd.Location = new System.Drawing.Point(250, 88);
            this.lbAnd.Name = "lbAnd";
            this.lbAnd.Size = new System.Drawing.Size(37, 25);
            this.lbAnd.TabIndex = 26;
            this.lbAnd.Text = "and";
            this.lbAnd.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbMadeBy
            // 
            this.lbMadeBy.BackColor = System.Drawing.Color.Transparent;
            this.lbMadeBy.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMadeBy.ForeColor = System.Drawing.Color.White;
            this.lbMadeBy.Location = new System.Drawing.Point(24, 88);
            this.lbMadeBy.Name = "lbMadeBy";
            this.lbMadeBy.Size = new System.Drawing.Size(189, 25);
            this.lbMadeBy.TabIndex = 25;
            this.lbMadeBy.Text = "made by";
            this.lbMadeBy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbMiou
            // 
            this.lbMiou.BackColor = System.Drawing.Color.Transparent;
            this.lbMiou.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMiou.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(202)))), ((int)(((byte)(232)))));
            this.lbMiou.Location = new System.Drawing.Point(208, 88);
            this.lbMiou.Name = "lbMiou";
            this.lbMiou.Size = new System.Drawing.Size(53, 25);
            this.lbMiou.TabIndex = 24;
            this.lbMiou.Text = "miou";
            this.lbMiou.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pbBird
            // 
            this.pbBird.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pbBird.BackColor = System.Drawing.Color.Transparent;
            this.pbBird.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbBird.Image = global::EEmulatorLauncher.Properties.Resources._196_LobbyState_twitter_LobbyState_twitter;
            this.pbBird.Location = new System.Drawing.Point(212, 11);
            this.pbBird.Name = "pbBird";
            this.pbBird.Size = new System.Drawing.Size(34, 43);
            this.pbBird.TabIndex = 23;
            this.pbBird.TabStop = false;
            // 
            // lbHeader
            // 
            this.lbHeader.BackColor = System.Drawing.Color.Transparent;
            this.lbHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbHeader.ForeColor = System.Drawing.Color.White;
            this.lbHeader.Location = new System.Drawing.Point(12, 63);
            this.lbHeader.Name = "lbHeader";
            this.lbHeader.Size = new System.Drawing.Size(440, 25);
            this.lbHeader.TabIndex = 22;
            this.lbHeader.Text = "Everybody Edits Emulator";
            this.lbHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AboutDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.ClientSize = new System.Drawing.Size(464, 248);
            this.Controls.Add(this.linkSource);
            this.Controls.Add(this.linkForums);
            this.Controls.Add(this.pbHeart);
            this.Controls.Add(this.lbThanksEveryone);
            this.Controls.Add(this.lbJesse);
            this.Controls.Add(this.lbAnd);
            this.Controls.Add(this.lbMadeBy);
            this.Controls.Add(this.lbMiou);
            this.Controls.Add(this.pbBird);
            this.Controls.Add(this.lbHeader);
            this.Controls.Add(this.pbConfetti);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(480, 287);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(480, 287);
            this.Name = "AboutDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.pbConfetti)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbHeart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBird)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pbConfetti;
        private System.Windows.Forms.LinkLabel linkSource;
        private System.Windows.Forms.LinkLabel linkForums;
        private System.Windows.Forms.PictureBox pbHeart;
        private System.Windows.Forms.Label lbThanksEveryone;
        private System.Windows.Forms.Label lbJesse;
        private System.Windows.Forms.Label lbAnd;
        private System.Windows.Forms.Label lbMadeBy;
        private System.Windows.Forms.Label lbMiou;
        private System.Windows.Forms.PictureBox pbBird;
        private System.Windows.Forms.Label lbHeader;
    }
}