using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace ICFP08
{
    public class StupidController : RoverController
    {
        public new static string Name
        {
            get
            {
                return "StupidController";
            }
        }
        private Vector2d m_target;
        private float m_desiredHeading;
        private float m_minAngle = 0.1f;
        private float m_fastAngle = 15.0f;
        private int m_maxTries = 18; // search 90 degrees each way
        private float m_avoidAngle = 5.0f;
        private int m_turnStop = 0; // used to time a turn
        private TurnType m_pendingTurn = TurnType.Straight;

        public override Vector2d CurrentTarget
        {
            get
            {
                return m_target;
            }
        }
        public override float DesiredHeading
        {
            get
            {
                return m_desiredHeading;
            }
        }

        public StupidController(WorldState world, ServerWrapper server)
            : base(world, server)
        {
            m_server.EndOfRunMessage += new ServerWrapper.EndOfRunMessageEventHandler(m_server_EndOfRunMessage);
        }

        void m_server_EndOfRunMessage(object sender, EndOfRunMessageEventArgs ee)
        {
            m_pendingTurn = TurnType.Straight;
        }

        public override void DoUpdate()
        {
            base.DoUpdate();
            m_target = new Vector2d(); // try to get to the center of the map if the home hasn't been found
            if (m_world.FoundHome)
                m_target = m_world.Home.Position;
            Vector2d offset = m_target - m_world.Rover.Position;
            m_desiredHeading = -(float)(Math.Atan2(offset.y, offset.x) * (180.0f / Math.PI)) + 90.0f;
            if (m_desiredHeading < 0.0f)
                m_desiredHeading += 360.0f;
            if (m_desiredHeading > 360.0f)
                m_desiredHeading -= 360.0f;

            // check for potential collisions
            for(int num_tries = 0; num_tries < m_maxTries; num_tries++)
            {
                float angle_offset = (float)num_tries * m_avoidAngle;
                if (!HitsSomething(m_desiredHeading + angle_offset)) // check right
                {
                    m_desiredHeading += angle_offset;
                    break;
                }
                if (!HitsSomething(m_desiredHeading - angle_offset)) // check left
                {
                    m_desiredHeading -= angle_offset;
                    break;
                }                
            }

            float turn_angle = m_desiredHeading - m_world.Rover.Direction;
            if (turn_angle > 180.0f)
                turn_angle = -360.0f + turn_angle;
            if (Math.Abs(turn_angle) > m_minAngle) // need to execute a turn
            {
                if (turn_angle > 0.0f) // need to turn right
                {
                    if (Math.Abs(turn_angle) > m_fastAngle) // turn hard
                    {
                        if (m_pendingTurn != TurnType.HardRight)
                        {
                            m_server.SendCommand(MoveType.Accelerate, TurnType.Right);
                            m_server.SendCommand(MoveType.Accelerate, TurnType.Right);
                            m_server.SendCommand(MoveType.Accelerate, TurnType.Right);
                            m_server.SendCommand(MoveType.Accelerate, TurnType.Right);
                            m_pendingTurn = TurnType.HardRight;
                        }
                    }
                    else
                    {
                        if (m_pendingTurn != TurnType.Right)
                        {
                            if (m_pendingTurn == TurnType.HardLeft)
                            {
                                m_server.SendCommand(MoveType.Accelerate, TurnType.Right);
                                m_server.SendCommand(MoveType.Accelerate, TurnType.Right);
                                m_server.SendCommand(MoveType.Accelerate, TurnType.Right);
                            }
                            else if (m_pendingTurn == TurnType.Left)
                            {
                                m_server.SendCommand(MoveType.Accelerate, TurnType.Right);
                                m_server.SendCommand(MoveType.Accelerate, TurnType.Right);
                            }
                            else if (m_pendingTurn == TurnType.Straight)
                                m_server.SendCommand(MoveType.Accelerate, TurnType.Right);
                            else if (m_pendingTurn == TurnType.HardRight)
                                m_server.SendCommand(MoveType.Accelerate, TurnType.Left);

                            m_pendingTurn = TurnType.Right;
                        }
                    }
                }
                else
                {
                    if (Math.Abs(turn_angle) > m_fastAngle) // turn hard
                    {
                        if (m_pendingTurn != TurnType.HardLeft)
                        {
                            m_server.SendCommand(MoveType.Accelerate, TurnType.Left);
                            m_server.SendCommand(MoveType.Accelerate, TurnType.Left);
                            m_server.SendCommand(MoveType.Accelerate, TurnType.Left);
                            m_server.SendCommand(MoveType.Accelerate, TurnType.Left);
                            m_pendingTurn = TurnType.HardLeft;
                        }
                    }
                    else
                    {
                        if (m_pendingTurn != TurnType.Left)
                        {
                            if (m_pendingTurn == TurnType.HardRight)
                            {
                                m_server.SendCommand(MoveType.Accelerate, TurnType.Left);
                                m_server.SendCommand(MoveType.Accelerate, TurnType.Left);
                                m_server.SendCommand(MoveType.Accelerate, TurnType.Left);
                            }
                            else if (m_pendingTurn == TurnType.Right)
                            {
                                m_server.SendCommand(MoveType.Accelerate, TurnType.Left);
                                m_server.SendCommand(MoveType.Accelerate, TurnType.Left);
                            }
                            else if (m_pendingTurn == TurnType.Straight)
                                m_server.SendCommand(MoveType.Accelerate, TurnType.Left);
                            else if(m_pendingTurn == TurnType.HardLeft)
                                m_server.SendCommand(MoveType.Accelerate, TurnType.Right);

                            m_pendingTurn = TurnType.Left;
                        }
                    }
                }
            }
            else
            {
                TurnType turn_command = TurnType.Straight;
                if (m_pendingTurn == TurnType.Right)
                    turn_command = TurnType.Left;
                else if (m_pendingTurn == TurnType.Left)
                    turn_command = TurnType.Right;
                m_server.SendCommand(MoveType.Accelerate, turn_command); // put the pedal to the metal
                m_pendingTurn = TurnType.Straight;
            }
        }

        private bool HitsSomething(float m_desiredHeading)
        {
            //m_desiredHeading = 360.0f;
            float length = (m_world.Rover.Position - m_target).length();
            Vector2d angle = new Vector2d(
                (float)Math.Cos(((-1.0f * m_desiredHeading) - 90) * (Math.PI / 180.0f)), 
                (float)Math.Sin(((-1.0f * m_desiredHeading) - 90) * (Math.PI / 180.0f)));
            Vector2d end = m_world.Rover.Position + (-length * angle);

            //DrawDebugLine(m_world.Rover.Position, end);

            foreach (Boulder b in m_world.Boulders)
                if (b.IntersectsLine(m_world.Rover.Position, end, 1.0f))
                    return true;

            foreach (Crater c in m_world.Craters)
                if (c.IntersectsLine(m_world.Rover.Position, end, 1.0f))
                    return true;

            return false;
        }
    }
}
