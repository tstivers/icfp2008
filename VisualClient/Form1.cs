using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ICFP08
{
    public partial class Form1 : Form
    {
        private ServerWrapper m_wrapper = new ServerWrapper();

        public Form1()
        {
            InitializeComponent();
            m_wrapper.InitializationMessage += new ServerWrapper.InitializationMessageEventHandler(m_wrapper_InitializationMessage);
            m_wrapper.TelemetryMessage += new ServerWrapper.TelemetryMessageEventHandler(m_wrapper_TelemetryMessage);
        }

        void m_wrapper_TelemetryMessage(object sender, TelemetryMessageEventArgs tme)
        {
            numericStatus.X = tme.xpos;
            numericStatus.Y = tme.ypos;
            numericStatus.Speed = tme.speed;
            numericStatus.Direction = tme.direction;
        }

        void m_wrapper_InitializationMessage(object sender, InitializationMessageEventArgs ime)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {

        }

        private void toolStripContainer2_RightToolStripPanel_Click(object sender, EventArgs e)
        {
                    }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_wrapper.Connect("172.16.1.44", 17676);
        }
    }
}