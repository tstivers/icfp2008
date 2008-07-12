using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ICFP08
{
    public partial class NumericStatusControl : UserControl
    {
        public float X
        {
            set
            {
                xBox.Text = value.ToString();
            }
        }

        public float Y
        {
            set
            {
                yBox.Text = value.ToString();
            }
        }
        
        public float Speed
        {
            set
            {
                speedBox.Text = value.ToString();
            }
        }

        public float Direction
        {
            set
            {
                angleBox.Text = value.ToString();
            }
        }

        public NumericStatusControl()
        {
            InitializeComponent();
        }
    }
}
