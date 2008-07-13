using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ICFP08
{
    public partial class CompassControl : UserControl
    {
        private float m_angle = 0.0f;
        private float m_wanted = 0.0f;

        public float Direction
        {
            set
            {
                m_angle = value;
                compassBox.Invalidate();
            }
        }

        public float WantedAngle
        {
            set
            {
                m_wanted = value;
                compassBox.Invalidate();
            }
        }

        public CompassControl()
        {
            InitializeComponent();
        }

        private void compassBox_Paint(object sender, PaintEventArgs e)
        {
            float x = (compassBox.Width / 2) + (float)Math.Sin(m_angle * (Math.PI / 180.0f)) * 100;
            float y = (compassBox.Height / 2) - (float)Math.Cos(m_angle * (Math.PI / 180.0f)) * 100;
            e.Graphics.DrawLine(Pens.Blue, new Point(compassBox.Width / 2, compassBox.Height / 2), new Point((int)x, (int)y));
            x = (compassBox.Width / 2) + (float)Math.Sin(m_wanted * (Math.PI / 180.0f)) * 100;
            y = (compassBox.Height / 2) - (float)Math.Cos(m_wanted * (Math.PI / 180.0f)) * 100;
            e.Graphics.DrawLine(Pens.Red, new Point(compassBox.Width / 2, compassBox.Height / 2), new Point((int)x, (int)y));
        }
    }
}
