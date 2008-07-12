using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ICFP08
{
    public partial class ClientForm : Form
    {
        private ServerWrapper m_wrapper = new ServerWrapper();
        private Timer m_timer = new Timer();
        private WorldState m_worldState; 

        public ClientForm()
        {
            InitializeComponent();
            m_wrapper.InitializationMessage += new ServerWrapper.InitializationMessageEventHandler(m_wrapper_InitializationMessage);
            m_wrapper.TelemetryMessage += new ServerWrapper.TelemetryMessageEventHandler(m_wrapper_TelemetryMessage);
            m_timer.Interval = 1;
            m_timer.Tick += new EventHandler(m_timer_Tick);
            m_timer.Start();
            roverControlStatus1.WantedMoveChanged += new RoverControlStatus.WantedMoveChangedHandler(roverControlStatus1_WantedMoveChanged);
            roverControlStatus1.WantedTurnChanged += new RoverControlStatus.WantedTurnChangedHandler(roverControlStatus1_WantedTurnChanged);
        }

        public void AddMessage(string message)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate {
                    AddMessage(message);
                }));
                return;
            }
            
            messageBox.AppendText(message + "\r\n");
        }

        void roverControlStatus1_WantedTurnChanged(object sender, RoverControlStatus.WantedTurnChangedArgs wtc)
        {
            m_wrapper.SendCommand(MoveType.Roll, wtc.state);
        }

        void roverControlStatus1_WantedMoveChanged(object sender, RoverControlStatus.WantedMoveChangedArgs wmc)
        {
            m_wrapper.SendCommand(wmc.state, TurnType.Straight); 
        }

        void m_timer_Tick(object sender, EventArgs e)
        {
            m_wrapper.ProcessMessages();
        }

        void m_wrapper_TelemetryMessage(object sender, TelemetryMessageEventArgs tme)
        {
            m_worldState.UpdateWorldState(tme.message);
            numericStatus.X = tme.message.position.x;
            numericStatus.Y = tme.message.position.y;
            numericStatus.Speed = tme.message.speed;
            numericStatus.Direction = tme.message.direction;
            compassControl.Direction = tme.message.direction;
            roverControlStatus1.MoveState = tme.message.move_state;
            roverControlStatus1.TurnState = tme.message.turn_state;
            timeLabel.Text = tme.message.time_stamp.ToString();
        }

        void m_wrapper_InitializationMessage(object sender, InitializationMessageEventArgs ime)
        {
            AddMessage("[wrapper] received init message (" + ime.message.size.Width + ", " + ime.message.size.Height + ")");
            m_worldState = new WorldState(ime.message);
            worldVisualizer.State = m_worldState;
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_wrapper.Connect(Properties.Settings.Default.server, Properties.Settings.Default.port);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settings = new SettingsForm();
            settings.serverBox.Text = Properties.Settings.Default.server;
            settings.portBox.Text = Properties.Settings.Default.port.ToString();

            if (settings.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.server = settings.serverBox.Text;
                Properties.Settings.Default.port = int.Parse(settings.portBox.Text);
                Properties.Settings.Default.Save();
            }
        }
    }
}