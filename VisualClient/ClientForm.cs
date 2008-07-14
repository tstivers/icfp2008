using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ICFP08
{
    public partial class ClientForm : Form
    {
        private ServerWrapper m_wrapper = new ServerWrapper();
        private Timer m_timer = new Timer();
        private WorldState m_worldState;
        private RoverController m_controller;

        public ClientForm()
        {
            InitializeComponent();
            m_wrapper.InitializationMessage += new ServerWrapper.InitializationMessageEventHandler(m_wrapper_InitializationMessage);
            m_wrapper.TelemetryMessage += new ServerWrapper.TelemetryMessageEventHandler(m_wrapper_TelemetryMessage);
            m_wrapper.EndOfRunMessage += new ServerWrapper.EndOfRunMessageEventHandler(m_wrapper_EndOfRunMessage);
            m_wrapper.KilledMessage += new ServerWrapper.EventMessageEventHandler(m_wrapper_KilledMessage);
            m_wrapper.SuccessMessage += new ServerWrapper.EventMessageEventHandler(m_wrapper_SuccessMessage);
            m_wrapper.CraterMessage += new ServerWrapper.EventMessageEventHandler(m_wrapper_CraterMessage);
            m_wrapper.CrashMessage += new ServerWrapper.EventMessageEventHandler(m_wrapper_CrashMessage);
            m_timer.Interval = 1;
            m_timer.Tick += new EventHandler(m_timer_Tick);
            m_timer.Start();
            roverControlStatus1.WantedMoveChanged += new RoverControlStatus.WantedMoveChangedHandler(roverControlStatus1_WantedMoveChanged);
            roverControlStatus1.WantedTurnChanged += new RoverControlStatus.WantedTurnChangedHandler(roverControlStatus1_WantedTurnChanged);
            worldVisualizer.MapClicked += new WorldVisualizer.MapClickedHandler(worldVisualizer_MapClicked);
        }

        void worldVisualizer_MapClicked(Vector2d position)
        {
            if (m_controller != null)
            {
                m_controller.CurrentTarget = position;
                m_controller.Flags |= RoverController.DebugFlags.ChooseRandomTarget;
            }
        }

        void m_wrapper_CrashMessage(object sender, EventMessageEventArgs ae)
        {
            AddMessage("[wrapper] Rover hit a boulder/border!");
        }

        void m_wrapper_CraterMessage(object sender, EventMessageEventArgs ae)
        {
            AddMessage("[wrapper] Fell into a crater!");
        }

        void m_wrapper_SuccessMessage(object sender, EventMessageEventArgs ae)
        {
            AddMessage("[wrapper] Successfully made it home!");
        }

        void m_wrapper_KilledMessage(object sender, EventMessageEventArgs ae)
        {
            AddMessage("[wrapper] Killed by a martian!");
        }

        void m_wrapper_EndOfRunMessage(object sender, EndOfRunMessageEventArgs ee)
        {
            AddMessage("[wrapper] Run ended: Score " + ee.score);
            m_worldState.ResetWorldState();
            aiStatsViewer1.Reset();
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
            if(!((m_worldState.Rover.Position.x == 0.0f) && (m_worldState.Rover.Position.y == 0.0f)))
                worldVisualizer.DrawLine(m_worldState.Rover.Position, tme.message.position, Pens.Transparent);
            m_worldState.UpdateWorldState(tme.message);
            Stopwatch timer = Stopwatch.StartNew();
            m_controller.DoUpdate();
            aiStatsViewer1.Time = (float)timer.Elapsed.TotalMilliseconds - (float)m_controller.SpinTime;
            numericStatus.X = tme.message.position.x;
            numericStatus.Y = tme.message.position.y;
            numericStatus.Speed = tme.message.speed;
            numericStatus.Direction = tme.message.direction;
            compassControl.Direction = tme.message.direction;
            compassControl.WantedAngle = m_controller.DesiredHeading;
            float turn_angle = m_controller.DesiredHeading - m_worldState.Rover.Direction;
            if (turn_angle > 180.0f)
                turn_angle = -360.0f + turn_angle;
            if (turn_angle < -180.0f)
                turn_angle = 360.0f + turn_angle;
            compassControl.OffsetAngle = turn_angle;
            roverControlStatus1.MoveState = tme.message.move_state;
            roverControlStatus1.TurnState = tme.message.turn_state;
            timeLabel.Text = tme.message.time_stamp.ToString();
            aiStatsViewer1.Casts = m_controller.NumRayCasts;
            aiStatsViewer1.Tests = m_controller.NumRayTests;
        }

        void m_wrapper_InitializationMessage(object sender, InitializationMessageEventArgs ime)
        {
            AddMessage("[wrapper] received init message (" + ime.message.size.Width + ", " + ime.message.size.Height + ")");
            m_worldState = new WorldState(ime.message);
            worldVisualizer.State = m_worldState;
            m_worldState.QuadTree.Collision += new SimpleQuadTree.CollisionHandler(QuadTree_Collision);
            m_controller = new StupidController(m_worldState, m_wrapper);
            m_controller.DebugLine += new RoverController.DebugLineHandler(m_controller_DebugLine);
            m_controller.DebugEllipse += new RoverController.DebugEllipseHandler(m_controller_DebugEllipse);
            m_controller.LogMessage += new RoverController.LogMessageHandler(m_controller_LogMessage);
            //m_controller.Flags |= RoverController.DebugFlags.DrawProximity;
            //m_controller.Flags |= RoverController.DebugFlags.DrawRays;
            //m_controller.Flags |= RoverController.DebugFlags.ChooseRandomTarget;
        }

        void QuadTree_Collision(Vector2d point)
        {
            worldVisualizer.DrawPoint(point, Brushes.Red);
        }

        void m_controller_DebugEllipse(MarsObject obj, Brush b)
        {
            worldVisualizer.DrawEllipse(obj, b);
        }

        void m_controller_LogMessage(string message)
        {
            AddMessage("[controller] " + message);
        }

        void m_controller_DebugLine(Vector2d start, Vector2d end, Pen pen)
        {
            worldVisualizer.DrawLine(start, end, pen);
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