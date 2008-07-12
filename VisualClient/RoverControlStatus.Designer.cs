namespace ICFP08
{
    partial class RoverControlStatus
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
            this.accellButton = new System.Windows.Forms.Button();
            this.coastButton = new System.Windows.Forms.Button();
            this.brakeButton = new System.Windows.Forms.Button();
            this.hardLeftButton = new System.Windows.Forms.Button();
            this.leftButton = new System.Windows.Forms.Button();
            this.straightButton = new System.Windows.Forms.Button();
            this.rightButton = new System.Windows.Forms.Button();
            this.hardRightButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // accellButton
            // 
            this.accellButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.accellButton.Location = new System.Drawing.Point(38, 3);
            this.accellButton.Name = "accellButton";
            this.accellButton.Size = new System.Drawing.Size(99, 23);
            this.accellButton.TabIndex = 0;
            this.accellButton.Text = "accelerate";
            this.accellButton.UseVisualStyleBackColor = true;
            this.accellButton.Click += new System.EventHandler(this.moveButton_Click);
            // 
            // coastButton
            // 
            this.coastButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.coastButton.Location = new System.Drawing.Point(38, 32);
            this.coastButton.Name = "coastButton";
            this.coastButton.Size = new System.Drawing.Size(99, 23);
            this.coastButton.TabIndex = 1;
            this.coastButton.Text = "coast";
            this.coastButton.UseVisualStyleBackColor = true;
            this.coastButton.Click += new System.EventHandler(this.moveButton_Click);
            // 
            // brakeButton
            // 
            this.brakeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.brakeButton.Location = new System.Drawing.Point(38, 61);
            this.brakeButton.Name = "brakeButton";
            this.brakeButton.Size = new System.Drawing.Size(99, 23);
            this.brakeButton.TabIndex = 2;
            this.brakeButton.Text = "brake";
            this.brakeButton.UseVisualStyleBackColor = true;
            this.brakeButton.Click += new System.EventHandler(this.moveButton_Click);
            // 
            // hardLeftButton
            // 
            this.hardLeftButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.hardLeftButton.Location = new System.Drawing.Point(3, 90);
            this.hardLeftButton.Name = "hardLeftButton";
            this.hardLeftButton.Size = new System.Drawing.Size(29, 23);
            this.hardLeftButton.TabIndex = 3;
            this.hardLeftButton.Text = "L";
            this.hardLeftButton.UseVisualStyleBackColor = true;
            this.hardLeftButton.Click += new System.EventHandler(this.turnButton_Click);
            // 
            // leftButton
            // 
            this.leftButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.leftButton.Location = new System.Drawing.Point(38, 90);
            this.leftButton.Name = "leftButton";
            this.leftButton.Size = new System.Drawing.Size(29, 23);
            this.leftButton.TabIndex = 4;
            this.leftButton.Text = "l";
            this.leftButton.UseVisualStyleBackColor = true;
            this.leftButton.Click += new System.EventHandler(this.turnButton_Click);
            // 
            // straightButton
            // 
            this.straightButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.straightButton.Location = new System.Drawing.Point(73, 90);
            this.straightButton.Name = "straightButton";
            this.straightButton.Size = new System.Drawing.Size(29, 23);
            this.straightButton.TabIndex = 5;
            this.straightButton.Text = "s";
            this.straightButton.UseVisualStyleBackColor = true;
            this.straightButton.Click += new System.EventHandler(this.turnButton_Click);
            // 
            // rightButton
            // 
            this.rightButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rightButton.Location = new System.Drawing.Point(108, 90);
            this.rightButton.Name = "rightButton";
            this.rightButton.Size = new System.Drawing.Size(29, 23);
            this.rightButton.TabIndex = 6;
            this.rightButton.Text = "r";
            this.rightButton.UseVisualStyleBackColor = true;
            this.rightButton.Click += new System.EventHandler(this.turnButton_Click);
            // 
            // hardRightButton
            // 
            this.hardRightButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.hardRightButton.Location = new System.Drawing.Point(143, 90);
            this.hardRightButton.Name = "hardRightButton";
            this.hardRightButton.Size = new System.Drawing.Size(29, 23);
            this.hardRightButton.TabIndex = 7;
            this.hardRightButton.Text = "R";
            this.hardRightButton.UseVisualStyleBackColor = true;
            this.hardRightButton.Click += new System.EventHandler(this.turnButton_Click);
            // 
            // RoverControlStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.hardRightButton);
            this.Controls.Add(this.rightButton);
            this.Controls.Add(this.straightButton);
            this.Controls.Add(this.leftButton);
            this.Controls.Add(this.hardLeftButton);
            this.Controls.Add(this.brakeButton);
            this.Controls.Add(this.coastButton);
            this.Controls.Add(this.accellButton);
            this.Name = "RoverControlStatus";
            this.Size = new System.Drawing.Size(178, 121);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button accellButton;
        private System.Windows.Forms.Button coastButton;
        private System.Windows.Forms.Button brakeButton;
        private System.Windows.Forms.Button hardLeftButton;
        private System.Windows.Forms.Button leftButton;
        private System.Windows.Forms.Button straightButton;
        private System.Windows.Forms.Button rightButton;
        private System.Windows.Forms.Button hardRightButton;
    }
}
