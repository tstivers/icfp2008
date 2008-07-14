namespace ICFP08
{
    partial class AIStatsViewer
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.timeLabel = new System.Windows.Forms.Label();
            this.testsLabel = new System.Windows.Forms.Label();
            this.castsLabel = new System.Windows.Forms.Label();
            this.maxTimeLabel = new System.Windows.Forms.Label();
            this.maxTestsLabel = new System.Windows.Forms.Label();
            this.maxCastsLabel = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ray Casts:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Ray Tests:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Think Time:";
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.Location = new System.Drawing.Point(72, 26);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(13, 13);
            this.timeLabel.TabIndex = 8;
            this.timeLabel.Text = "0";
            // 
            // testsLabel
            // 
            this.testsLabel.AutoSize = true;
            this.testsLabel.Location = new System.Drawing.Point(72, 13);
            this.testsLabel.Name = "testsLabel";
            this.testsLabel.Size = new System.Drawing.Size(13, 13);
            this.testsLabel.TabIndex = 7;
            this.testsLabel.Text = "0";
            // 
            // castsLabel
            // 
            this.castsLabel.AutoSize = true;
            this.castsLabel.Location = new System.Drawing.Point(72, 0);
            this.castsLabel.Name = "castsLabel";
            this.castsLabel.Size = new System.Drawing.Size(13, 13);
            this.castsLabel.TabIndex = 6;
            this.castsLabel.Text = "0";
            // 
            // maxTimeLabel
            // 
            this.maxTimeLabel.AutoSize = true;
            this.maxTimeLabel.Location = new System.Drawing.Point(72, 102);
            this.maxTimeLabel.Name = "maxTimeLabel";
            this.maxTimeLabel.Size = new System.Drawing.Size(13, 13);
            this.maxTimeLabel.TabIndex = 14;
            this.maxTimeLabel.Text = "0";
            // 
            // maxTestsLabel
            // 
            this.maxTestsLabel.AutoSize = true;
            this.maxTestsLabel.Location = new System.Drawing.Point(72, 89);
            this.maxTestsLabel.Name = "maxTestsLabel";
            this.maxTestsLabel.Size = new System.Drawing.Size(13, 13);
            this.maxTestsLabel.TabIndex = 13;
            this.maxTestsLabel.Text = "0";
            // 
            // maxCastsLabel
            // 
            this.maxCastsLabel.AutoSize = true;
            this.maxCastsLabel.Location = new System.Drawing.Point(72, 76);
            this.maxCastsLabel.Name = "maxCastsLabel";
            this.maxCastsLabel.Size = new System.Drawing.Size(13, 13);
            this.maxCastsLabel.TabIndex = 12;
            this.maxCastsLabel.Text = "0";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 102);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(63, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Think Time:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 89);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "Ray Tests:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 76);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(58, 13);
            this.label12.TabIndex = 9;
            this.label12.Text = "Ray Casts:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 51);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(30, 13);
            this.label13.TabIndex = 15;
            this.label13.Text = "Max:";
            // 
            // AIStatsViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label13);
            this.Controls.Add(this.maxTimeLabel);
            this.Controls.Add(this.maxTestsLabel);
            this.Controls.Add(this.maxCastsLabel);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.testsLabel);
            this.Controls.Add(this.castsLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "AIStatsViewer";
            this.Size = new System.Drawing.Size(150, 121);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.Label testsLabel;
        private System.Windows.Forms.Label castsLabel;
        private System.Windows.Forms.Label maxTimeLabel;
        private System.Windows.Forms.Label maxTestsLabel;
        private System.Windows.Forms.Label maxCastsLabel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
    }
}
