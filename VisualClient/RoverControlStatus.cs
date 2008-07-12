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
        public class WantedMoveChangedArgs : EventArgs
        {
            public WantedMoveChangedArgs(MoveType state)
            {
                this.state = state;
            }

            public readonly MoveType state;
        }

        public class WantedTurnChangedArgs : EventArgs
        {
            public WantedTurnChangedArgs(TurnType state)
            {
                this.state = state;
            }

            public readonly TurnType state;
        }

        public delegate void WantedTurnChangedHandler(object sender, WantedTurnChangedArgs wtc);
        public event WantedTurnChangedHandler WantedTurnChanged;

        public delegate void WantedMoveChangedHandler(object sender, WantedMoveChangedArgs wmc);
        public event WantedMoveChangedHandler WantedMoveChanged;

        private Button m_currentMove = null;
        private Button m_currentTurn = null;
        private Button m_wantedMove = null;
        private Button m_wantedTurn = null;

        public MoveType MoveState
        {
            set
            {
                Button newButton = coastButton;
                switch (value)
                {
                    case MoveType.Accelerate:
                        newButton = accellButton;
                        break;
                    case MoveType.Brake:
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

        public TurnType TurnState
        {
            set
            {
                Button newButton = straightButton;
                switch (value)
                {
                    case TurnType.HardLeft:
                        newButton = hardLeftButton;
                        break;
                    case TurnType.Left:
                        newButton = leftButton;
                        break;
                    case TurnType.Right:
                        newButton = rightButton;
                        break;
                    case TurnType.HardRight:
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
            MoveState = MoveType.Roll;
            TurnState = TurnType.Straight;
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
                    m_wantedMove == accellButton ? MoveType.Accelerate :
                        m_wantedMove == coastButton ? MoveType.Roll : MoveType.Brake));
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
                    m_wantedTurn == hardLeftButton ? TurnType.HardLeft :
                    m_wantedTurn == leftButton ? TurnType.Left : 
                    m_wantedTurn == straightButton ? TurnType.Straight :
                    m_wantedTurn == rightButton ? TurnType.Right : TurnType.HardRight));
            }
        }  

    }
}
