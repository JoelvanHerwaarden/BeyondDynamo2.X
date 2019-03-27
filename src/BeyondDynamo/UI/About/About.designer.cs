namespace BeyondDynamo.UI.About
{
    partial class About
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
            this.LinkedIn_Label = new System.Windows.Forms.Label();
            this.GitHub_Label = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LinkedIn_Label
            // 
            this.LinkedIn_Label.AutoSize = true;
            this.LinkedIn_Label.BackColor = System.Drawing.Color.Transparent;
            this.LinkedIn_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LinkedIn_Label.ForeColor = System.Drawing.Color.White;
            this.LinkedIn_Label.Location = new System.Drawing.Point(119, 427);
            this.LinkedIn_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LinkedIn_Label.Name = "LinkedIn_Label";
            this.LinkedIn_Label.Size = new System.Drawing.Size(311, 17);
            this.LinkedIn_Label.TabIndex = 6;
            this.LinkedIn_Label.Text = "www.linkedin.com/in/Joel-van-Herwaarden";
            this.LinkedIn_Label.Click += new System.EventHandler(this.LinkedIn_Label_Click);
            this.LinkedIn_Label.MouseEnter += new System.EventHandler(this.LinkedIn_Label_MouseEnter);
            this.LinkedIn_Label.MouseLeave += new System.EventHandler(this.LinkedIn_Label_MouseLeave);
            // 
            // GitHub_Label
            // 
            this.GitHub_Label.AutoSize = true;
            this.GitHub_Label.BackColor = System.Drawing.Color.Transparent;
            this.GitHub_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GitHub_Label.ForeColor = System.Drawing.Color.White;
            this.GitHub_Label.Location = new System.Drawing.Point(666, 427);
            this.GitHub_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.GitHub_Label.Name = "GitHub_Label";
            this.GitHub_Label.Size = new System.Drawing.Size(209, 17);
            this.GitHub_Label.TabIndex = 7;
            this.GitHub_Label.Text = "Beyond Dynamo 2.X GitHub";
            this.GitHub_Label.Click += new System.EventHandler(this.GitHub_Label_Click);
            this.GitHub_Label.MouseEnter += new System.EventHandler(this.GitHub_Label_MouseEnter);
            this.GitHub_Label.MouseLeave += new System.EventHandler(this.GitHub_Label_MouseLeave);
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.BackColor = System.Drawing.Color.Transparent;
            this.versionLabel.ForeColor = System.Drawing.Color.Silver;
            this.versionLabel.Location = new System.Drawing.Point(1002, 462);
            this.versionLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(60, 13);
            this.versionLabel.TabIndex = 8;
            this.versionLabel.Text = "Build: 1.0.0";
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.BackgroundImage = global::BeyondDynamo.resources.BeyondDynamo_Banner;
            this.ClientSize = new System.Drawing.Size(1086, 495);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.GitHub_Label);
            this.Controls.Add(this.LinkedIn_Label);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About Beyond Dynamo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label LinkedIn_Label;
        private System.Windows.Forms.Label GitHub_Label;
        private System.Windows.Forms.Label versionLabel;
    }
}