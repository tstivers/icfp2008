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
            ((System.ComponentModel.ISupportInitialize)(this.compassBox)).BeginInit();
            this.SuspendLayout();
            // 
            // compassBox
            // 
            this.compassBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compassBox.Image = global::ICFP08.Properties.Resources.compass;
            this.compassBox.Location = new System.Drawing.Point(0, 0);
            this.compassBox.Name = "compassBox";
            this.compassBox.Size = new System.Drawing.Size(150, 150);
            this.compassBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.compassBox.TabIndex = 0;
            this.compassBox.TabStop = false;
            this.compassBox.Paint += new System.Windows.Forms.PaintEventHandler(this.compassBox_Paint);
            // 
            // CompassControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.compassBox);
            this.Name = "CompassControl";
            ((System.ComponentModel.ISupportInitialize)(this.compassBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox compassBox;
    }
}
