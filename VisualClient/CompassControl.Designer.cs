namespace ICFP08
{
    partial class CompassControl
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
            this.compassBox = new System.Windows.Forms.PictureBox();
            this.offsetLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.compassBox)).BeginInit();
            this.SuspendLayout();
            // 
            // compassBox
            // 
            this.compassBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compassBox.Location = new System.Drawing.Point(0, 0);
            this.compassBox.Name = "compassBox";
            this.compassBox.Size = new System.Drawing.Size(150, 150);
            this.compassBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.compassBox.TabIndex = 0;
            this.compassBox.TabStop = false;
            this.compassBox.Paint += new System.Windows.Forms.PaintEventHandler(this.compassBox_Paint);
            // 
            // offsetLabel
            // 
            this.offsetLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.offsetLabel.AutoSize = true;
            this.offsetLabel.Location = new System.Drawing.Point(74, 137);
            this.offsetLabel.Name = "offsetLabel";
            this.offsetLabel.Size = new System.Drawing.Size(25, 13);
            this.offsetLabel.TabIndex = 1;
            this.offsetLabel.Text = "0.0f";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "offset angle:";
            // 
            // CompassControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.offsetLabel);
            this.Controls.Add(this.compassBox);
            this.Name = "CompassControl";
            ((System.ComponentModel.ISupportInitialize)(this.compassBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox compassBox;
        private System.Windows.Forms.Label offsetLabel;
        private System.Windows.Forms.Label label2;
    }
}
