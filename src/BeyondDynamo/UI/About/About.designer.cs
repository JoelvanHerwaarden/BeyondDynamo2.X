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
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LinkedIn_Label
            // 
            this.LinkedIn_Label.AutoSize = true;
            this.LinkedIn_Label.BackColor = System.Drawing.Color.Transparent;
            this.LinkedIn_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LinkedIn_Label.ForeColor = System.Drawing.Color.White;
            this.LinkedIn_Label.Location = new System.Drawing.Point(117, 432);
            this.LinkedIn_Label.Name = "LinkedIn_Label";
            this.LinkedIn_Label.Size = new System.Drawing.Size(363, 20);
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
            this.GitHub_Label.Location = new System.Drawing.Point(669, 432);
            this.GitHub_Label.Name = "GitHub_Label";
            this.GitHub_Label.Size = new System.Drawing.Size(243, 20);
            this.GitHub_Label.TabIndex = 7;
            this.GitHub_Label.Text = "Beyond Dynamo 2.X GitHub";
            this.GitHub_Label.Click += new System.EventHandler(this.GitHub_Label_Click);
            this.GitHub_Label.MouseEnter += new System.EventHandler(this.GitHub_Label_MouseEnter);
            this.GitHub_Label.MouseLeave += new System.EventHandler(this.GitHub_Label_MouseLeave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.Silver;
            this.label2.Location = new System.Drawing.Point(995, 469);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Build: 1.0.0";
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.BackgroundImage = global::BeyondDynamo.resources.BeyondDynamo_Banner;
            this.ClientSize = new System.Drawing.Size(1086, 495);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.GitHub_Label);
            this.Controls.Add(this.LinkedIn_Label);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
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
        private System.Windows.Forms.Label label2;
    }
}