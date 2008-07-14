using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ICFP08
{
    public partial class AIStatsViewer : UserControl
    {
        private int m_numCasts;
        private int m_numTests;
        private float m_thinkTime;

        private int m_maxCasts;
        private int m_maxTests;
        private float m_maxThink;

        public int Casts
        {
            get
            {
                return m_numCasts;
            }
            set
            {
                castsLabel.Text = value.ToString();
                m_numCasts = value;
                if (m_numCasts > m_maxCasts)
                {
                    m_maxCasts = value;
                    maxCastsLabel.Text = value.ToString();
                }
            }
        }

        public int Tests
        {
            get
            {
                return m_numTests;
            }
            set
            {
                testsLabel.Text = value.ToString();
                m_numTests = value;
                if (value > m_maxTests)
                {
                    m_maxTests = value;
                    maxTestsLabel.Text = value.ToString();
                }
            }
        }

        public float Time
        {
            get
            {
                return m_thinkTime;
            }
            set
            {
                timeLabel.Text = value.ToString() + " ms";
                m_thinkTime = value;
                if (value > m_maxThink)
                {
                    m_maxThink = value;
                    maxTimeLabel.Text = value.ToString() + " ms";
                }
            }
        }

        public void Reset()
        {
            m_maxThink = 0;
            m_maxTests = 0;
            m_maxCasts = 0;
            m_numCasts = 0;
            m_numTests = 0;
            m_thinkTime = 0;
        }


        public AIStatsViewer()
        {
            InitializeComponent();
        }
    }
}
