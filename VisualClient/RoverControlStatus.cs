using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ICFP08
{
    public partial class RoverControlStatus : UserControl
    {
        public enum MOVE_STATE
        {
            accelerate,
            coast,
            brake
        }

        public enum TURN_STATE
        {
            hard_left,
            left,
            straight,
            right,
            hard_right
        }

        public class WantedMoveChangedArgs : EventArgs
        {
            public WantedMoveChangedArgs(MOVE_STATE state)
            {
                this.state = state;
            }

            public readonly MOVE_STATE state;
        }

        public class WantedTurnChangedArgs : EventArgs
        {
            public WantedTurnChangedArgs(TURN_STATE state)
            {
                this.state = state;
            }

            public readonly TURN_STATE state;
        }

        public delegate void WantedTurnChangedHandler(object sender, WantedTurnChangedArgs wtc);
        public event WantedTurnChangedHandler WantedTurnChanged;

        public delegate void WantedMoveChangedHandler(object sender, WantedMoveChangedArgs wmc);
        public event WantedMoveChangedHandler WantedMoveChanged;

        private Button m_currentMove = null;
        private Button m_currentTurn = null;
        private Button m_wantedMove = null;
        private Button m_wantedTurn = null;

        public MOVE_STATE MoveState
        {
            set
            {
                Button newButton = coastButton;
                switch (value)
                {
                    case MOVE_STATE.accelerate:
                        newButton = accellButton;
                        break;
                    case MOVE_STATE.brake:
                        newButton = brakeButton;
                        break;
                    default:
                        break;
                }
                if (m_wantedMove != null)
                {
                    m_wantedMove.BackColor = SystemColors.Control;
                    m_wantedMove = null;
                }

                if(m_currentMove != null)
                    m_currentMove.BackColor = SystemColors.Control;
                newButton.BackColor = SystemColors.HotTrack;
                m_currentMove = newButton;
            }
        }

        public TURN_STATE TurnState
        {
            set
            {
                Button newButton = straightButton;
                switch (value)
                {
                    case TURN_STATE.hard_left:
                        newButton = hardLeftButton;
                        break;
                    case TURN_STATE.left:
                        newButton = leftButton;
                        break;
                    case TURN_STATE.right:
                        newButton = rightButton;
                        break;
                    case TURN_STATE.hard_right:
                        newButton = hardRightButton;
                        break;
                    default:
                        break;
                }
                if (m_wantedTurn != null)
                {
                    m_wantedTurn.BackColor = SystemColors.Control;
                    m_wantedTurn = null;
                }
                if (m_currentTurn != null)
                    m_currentTurn.BackColor = SystemColors.Control;
                newButton.BackColor = SystemColors.HotTrack;
                m_currentTurn = newButton;
            }
        }
        
        public RoverControlStatus()
        {
            InitializeComponent();
            MoveState = MOVE_STATE.coast;
            TurnState = TURN_STATE.straight;
        }

        private void moveButton_Click(object sender, EventArgs e)
        {
            if (m_wantedMove != null && m_wantedMove.BackColor == SystemColors.Highlight)
                m_wantedMove.BackColor = SystemColors.Control;
            m_wantedMove = (Button)sender;
            m_wantedMove.BackColor = SystemColors.Highlight;
            if (WantedMoveChanged != null)
            {
                WantedMoveChanged(this, new WantedMoveChangedArgs(
                    m_wantedMove == accellButton ? MOVE_STATE.accelerate :
                        m_wantedMove == coastButton ? MOVE_STATE.coast : MOVE_STATE.brake));
            }
        }

        private void turnButton_Click(object sender, EventArgs e)
        {
            if (m_wantedTurn != null && m_wantedTurn.BackColor == SystemColors.Highlight)
                m_wantedTurn.BackColor = SystemColors.Control;
            m_wantedTurn = (Button)sender;
            m_wantedTurn.BackColor = SystemColors.Highlight;
            if (WantedTurnChanged != null)
            {
                WantedTurnChanged(this, new WantedTurnChangedArgs(
                    m_wantedTurn == hardLeftButton ? TURN_STATE.hard_left :
                       m_wantedTurn == leftButton ? TURN_STATE.left : 
                        m_wantedTurn == straightButton ? TURN_STATE.straight :
                        m_wantedTurn == rightButton ? TURN_STATE.right : TURN_STATE.hard_right));
            }
        }  

    }
}
