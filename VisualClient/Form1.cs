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
        private Timer m_timer = new Timer();

        public Form1()
        {
            InitializeComponent();
            m_wrapper.InitializationMessage += new ServerWrapper.InitializationMessageEventHandler(m_wrapper_InitializationMessage);
            m_wrapper.TelemetryMessage += new ServerWrapper.TelemetryMessageEventHandler(m_wrapper_TelemetryMessage);
            m_timer.Interval = 10;
            m_timer.Tick += new EventHandler(m_timer_Tick);
            m_timer.Start();
            roverControlStatus1.WantedMoveChanged += new RoverControlStatus.WantedMoveChangedHandler(roverControlStatus1_WantedMoveChanged);
            roverControlStatus1.WantedTurnChanged += new RoverControlStatus.WantedTurnChangedHandler(roverControlStatus1_WantedTurnChanged);
        }

        void roverControlStatus1_WantedTurnChanged(object sender, RoverControlStatus.WantedTurnChangedArgs wtc)
        {
            m_wrapper.SendCommand(MOVE_STATE.rolling,
                wtc.state == RoverControlStatus.TURN_STATE.hard_left || wtc.state == RoverControlStatus.TURN_STATE.left ? TURN_STATE.left :
                wtc.state == RoverControlStatus.TURN_STATE.hard_right || wtc.state == RoverControlStatus.TURN_STATE.right ? TURN_STATE.right :
                TURN_STATE.straight);
        }

        void roverControlStatus1_WantedMoveChanged(object sender, RoverControlStatus.WantedMoveChangedArgs wmc)
        {
            m_wrapper.SendCommand(wmc.state == RoverControlStatus.MOVE_STATE.accelerate ? MOVE_STATE.accelerating :
                wmc.state == RoverControlStatus.MOVE_STATE.brake ? MOVE_STATE.braking : MOVE_STATE.rolling,
                TURN_STATE.straight);
        }

        void m_timer_Tick(object sender, EventArgs e)
        {
            m_wrapper.ProcessMessages();
        }

        void m_wrapper_TelemetryMessage(object sender, TelemetryMessageEventArgs tme)
        {
            numericStatus.X = tme.xpos;
            numericStatus.Y = tme.ypos;
            numericStatus.Speed = tme.speed;
            numericStatus.Direction = tme.direction;
            compassControl.Direction = tme.direction;
            roverControlStatus1.MoveState = tme.move_state == MOVE_STATE.accelerating ? RoverControlStatus.MOVE_STATE.accelerate :
                tme.move_state == MOVE_STATE.braking ? RoverControlStatus.MOVE_STATE.brake : RoverControlStatus.MOVE_STATE.coast;
            roverControlStatus1.TurnState = tme.turn_state == TURN_STATE.hard_left ? RoverControlStatus.TURN_STATE.hard_left :
                tme.turn_state == TURN_STATE.left ? RoverControlStatus.TURN_STATE.left :
                tme.turn_state == TURN_STATE.straight ? RoverControlStatus.TURN_STATE.straight :
                tme.turn_state == TURN_STATE.right ? RoverControlStatus.TURN_STATE.right : RoverControlStatus.TURN_STATE.hard_right;
        }

        void m_wrapper_InitializationMessage(object sender, InitializationMessageEventArgs ime)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_wrapper.Connect("172.16.1.44", 17676);
        }
    }
}