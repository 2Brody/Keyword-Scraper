namespace Keyword_Scraper_By_MasterC
{
    partial class FrmJoin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmJoin));
            this.lblTextShow = new System.Windows.Forms.Label();
            this.btnJoin = new System.Windows.Forms.Button();
            this.btnNotNow = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTextShow
            // 
            this.lblTextShow.AutoSize = true;
            this.lblTextShow.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTextShow.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblTextShow.Location = new System.Drawing.Point(88, 83);
            this.lblTextShow.Name = "lblTextShow";
            this.lblTextShow.Size = new System.Drawing.Size(25, 21);
            this.lblTextShow.TabIndex = 11;
            this.lblTextShow.Text = "Jo";
            // 
            // btnJoin
            // 
            this.btnJoin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.btnJoin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJoin.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnJoin.Location = new System.Drawing.Point(139, 144);
            this.btnJoin.Name = "btnJoin";
            this.btnJoin.Size = new System.Drawing.Size(75, 25);
            this.btnJoin.TabIndex = 10;
            this.btnJoin.UseVisualStyleBackColor = false;
            this.btnJoin.Click += new System.EventHandler(this.btnJoin_Click);
            this.btnJoin.MouseEnter += new System.EventHandler(this.btnJoin_MouseEnter);
            this.btnJoin.MouseLeave += new System.EventHandler(this.btnJoin_MouseLeave);
            // 
            // btnNotNow
            // 
            this.btnNotNow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.btnNotNow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNotNow.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnNotNow.Location = new System.Drawing.Point(231, 144);
            this.btnNotNow.Name = "btnNotNow";
            this.btnNotNow.Size = new System.Drawing.Size(75, 25);
            this.btnNotNow.TabIndex = 9;
            this.btnNotNow.Text = "Not Now";
            this.btnNotNow.UseVisualStyleBackColor = false;
            this.btnNotNow.Click += new System.EventHandler(this.btnNotNow_Click);
            this.btnNotNow.MouseEnter += new System.EventHandler(this.btnNotNow_MouseEnter_1);
            this.btnNotNow.MouseLeave += new System.EventHandler(this.btnNotNow_MouseLeave);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 46);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(70, 113);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(420, 40);
            this.panel1.TabIndex = 2;
            // 
            // FrmJoin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(420, 200);
            this.Controls.Add(this.lblTextShow);
            this.Controls.Add(this.btnJoin);
            this.Controls.Add(this.btnNotNow);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmJoin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmJoin";
            this.Load += new System.EventHandler(this.FrmJoin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblTextShow;
        private System.Windows.Forms.Button btnJoin;
        private System.Windows.Forms.Button btnNotNow;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
    }
}