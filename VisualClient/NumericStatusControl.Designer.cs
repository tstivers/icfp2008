namespace ICFP08
{
    partial class NumericStatusControl
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
            this.xBox = new System.Windows.Forms.TextBox();
            this.speedBox = new System.Windows.Forms.TextBox();
            this.angleBox = new System.Windows.Forms.TextBox();
            this.yBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "pos";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "speed";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "angle";
            // 
            // xBox
            // 
            this.xBox.Location = new System.Drawing.Point(57, 14);
            this.xBox.Name = "xBox";
            this.xBox.Size = new System.Drawing.Size(52, 20);
            this.xBox.TabIndex = 3;
            // 
            // speedBox
            // 
            this.speedBox.Location = new System.Drawing.Point(57, 40);
            this.speedBox.Name = "speedBox";
            this.speedBox.Size = new System.Drawing.Size(110, 20);
            this.speedBox.TabIndex = 4;
            // 
            // angleBox
            // 
            this.angleBox.Location = new System.Drawing.Point(57, 66);
            this.angleBox.Name = "angleBox";
            this.angleBox.Size = new System.Drawing.Size(110, 20);
            this.angleBox.TabIndex = 5;
            // 
            // yBox
            // 
            this.yBox.Location = new System.Drawing.Point(115, 14);
            this.yBox.Name = "yBox";
            this.yBox.Size = new System.Drawing.Size(52, 20);
            this.yBox.TabIndex = 6;
            // 
            // NumericStatusControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.yBox);
            this.Controls.Add(this.angleBox);
            this.Controls.Add(this.speedBox);
            this.Controls.Add(this.xBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "NumericStatusControl";
            this.Size = new System.Drawing.Size(187, 105);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox xBox;
        private System.Windows.Forms.TextBox speedBox;
        private System.Windows.Forms.TextBox angleBox;
        private System.Windows.Forms.TextBox yBox;
    }
}
