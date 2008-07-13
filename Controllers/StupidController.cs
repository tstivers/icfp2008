using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Drawing;
using System.Diagnostics;

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
        private float m_minAngle = 1.0f;
        private float m_fastAngle = 20.0f;
        private float m_brakeAngle = 60.0f; // the angle at which we decelerate
        private float m_minBrakeSpeed = 8.0f; // assuming we are going faster than this
        private int m_maxTries = 18; // number of times to try and find a non-colliding heading
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

            // calc max distance for collisions
            float target_distance = (m_world.Rover.Position - m_target).length();
            float max_distance = Math.Min(target_distance, m_world.Rover.Speed * 3.0f);            

            // check for critical collisions on our current trajectory
            MarsObject critical_obj = null;
            float critical_dist = NearestObject(m_world.Rover.Direction, max_distance, ref critical_obj);

            // figure out a desired trajectory that doesn't run into anything
            int num_tries = 0;
            for (num_tries = 0; num_tries < m_maxTries; num_tries++)
            {
                float angle_offset = (float)num_tries * m_avoidAngle;
                MarsObject nearest_obj = null;

                if(float.MaxValue == NearestObject(m_desiredHeading + angle_offset, max_distance, ref nearest_obj)) // check right
                {
                    m_desiredHeading += angle_offset;
                    break;
                }

                if(float.MaxValue == NearestObject(m_desiredHeading - angle_offset, max_distance, ref nearest_obj)) // check left
                {
                    m_desiredHeading -= angle_offset;
                    break;
                }
            }

            // if we fail to find a decent trajectory, just keep going the way we're going and
            // rely on the critical avoidance to keep us safe while we look for a new trajectory
            if (num_tries == m_maxTries)
                m_desiredHeading = m_world.Rover.Direction;

            if (critical_obj != null) // it's coming right at us!
            {
                DrawDebugEllipse(critical_obj, Brushes.Yellow);
                //DrawDebugRay(m_world.Rover.Position, m_desiredHeading, max_distance, Pens.Red);
                //DrawDebugRay(m_world.Rover.Position, m_world.Rover.Direction, max_distance, Pens.Yellow);
                m_desiredHeading = FindBestHeadingWhileAvoidingThis(critical_obj, critical_dist);
                DrawDebugRay(m_world.Rover.Position, m_desiredHeading, max_distance, Pens.Green);
                DrawDebugRay(m_world.Rover.Position, m_world.Rover.Direction, max_distance, Pens.Yellow);
            }
            else // not in immediate danger
            {             
                //DrawDebugRay(m_world.Rover.Position, m_desiredHeading, max_distance, Pens.Blue);
                //DrawDebugRay(m_world.Rover.Position, m_world.Rover.Direction, max_distance, Pens.Green);                
            }

            //if (num_tries == m_maxTries) // PANIC : at worst we'll crash
            //{
            //    Log("PANIC");
            //    //MoveType t = m_world.Rover.Speed > m_minBrakeSpeed ? MoveType.Brake : MoveType.Accelerate;
            //    MoveType mt = MoveType.Brake;
            //    TurnType tt = (m_pendingTurn == TurnType.Right) || (m_pendingTurn == TurnType.HardRight) ? 
            //        TurnType.HardRight : TurnType.HardLeft;
            //    m_pendingTurn = tt;
            //    m_server.SendCommand(mt, tt);
            //    m_server.SendCommand(mt, tt);
            //    m_server.SendCommand(mt, tt);
            //    m_server.SendCommand(mt, tt);
            //}
            
            float turn_angle = m_desiredHeading - m_world.Rover.Direction;
            if (turn_angle > 180.0f)
                turn_angle = -360.0f + turn_angle;
            if (turn_angle < -180.0f)
                turn_angle = 360.0f - turn_angle;
            if (Math.Abs(turn_angle) > m_minAngle) // need to execute a turn
            {
                if (turn_angle > 0.0f) // need to turn right
                {
                    if (Math.Abs(turn_angle) > m_fastAngle) // turn hard
                    {
                        if (m_pendingTurn != TurnType.HardRight || !(offset.length() < m_world.Rover.Speed))
                        {
                            // avoid the SPIRAL OF DEATH
                            MoveType gas = (offset.length() < m_world.Rover.Speed) ? MoveType.Brake : MoveType.Accelerate;
                            if (gas == MoveType.Brake)
                                Log("SPIRALING: " + offset.length() + " < " + m_world.Rover.Speed);
                            m_server.SendCommand(gas, TurnType.Right);
                            m_server.SendCommand(gas, TurnType.Right);
                            m_server.SendCommand(gas, TurnType.Right);
                            m_server.SendCommand(gas, TurnType.Right);
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
                        if (m_pendingTurn != TurnType.HardLeft || !(offset.length() < m_world.Rover.Speed))
                        {
                            // avoid the SPIRAL OF DEATH
                            MoveType gas = (offset.length() < m_world.Rover.Speed) ? MoveType.Brake : MoveType.Accelerate;
                            if (gas == MoveType.Brake)
                                Log("SPIRALING: " + offset.length() + " < " + m_world.Rover.Speed);
                            m_server.SendCommand(gas, TurnType.Left);
                            m_server.SendCommand(gas, TurnType.Left);
                            m_server.SendCommand(gas, TurnType.Left);
                            m_server.SendCommand(gas, TurnType.Left);
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

        private float FindBestHeadingWhileAvoidingThis(MarsObject closest_obj, float distance)
        {
            for (float i = 0.0f; i < 90.0f; i++) // BRUTE FORCE, TODO: use point-distance-from-line formula
            {
                MarsObject new_obj = null;
                float new_dist;

                // check right
                new_dist = NearestObject(m_world.Rover.Direction + i, m_world.Rover.Speed * 3.0f, ref new_obj);
                if ((new_obj != closest_obj) && (new_dist > distance))
                    return m_world.Rover.Direction + i;

                // check left
                new_dist = NearestObject(m_world.Rover.Direction - i, m_world.Rover.Speed * 3.0f, ref new_obj);
                if ((new_obj != closest_obj) && (new_dist > distance))
                    return m_world.Rover.Direction - i;
            }

            // we are probably screwed
            Log("SCREWED");
            return m_world.Rover.Direction;
        }

        private float NearestObject(float m_desiredHeading, float max_dist, ref MarsObject closest_obj)
        {

            Vector2d angle = new Vector2d(
                (float)Math.Cos(((-1.0f * m_desiredHeading) - 90) * (Math.PI / 180.0f)), 
                (float)Math.Sin(((-1.0f * m_desiredHeading) - 90) * (Math.PI / 180.0f)));
            Vector2d end = m_world.Rover.Position + (-max_dist * angle);

            //DrawDebugLine(m_world.Rover.Position, end, Pens.Red);
            float closest_range = float.MaxValue;

            foreach (Boulder b in m_world.Boulders)
                if (b.IntersectsLine(m_world.Rover.Position, end, 0.5f))
                {
                    //DrawDebugEllipse(b, Brushes.Yellow);
                    if ((m_world.Rover.Position - b.Position).length() - b.Radius < closest_range)
                    {
                        closest_range = (m_world.Rover.Position - b.Position).length() - b.Radius;
                        closest_obj = b;
                    }
                }

            foreach (Crater c in m_world.Craters)
                if (c.IntersectsLine(m_world.Rover.Position, end, 0.5f))
                {
                    //DrawDebugEllipse(c, Brushes.Yellow);
                    if ((m_world.Rover.Position - c.Position).length() - c.Radius < closest_range)
                    {
                        closest_range = (m_world.Rover.Position - c.Position).length() - c.Radius;
                        closest_obj = c;
                    }
                }            

            foreach (Martian m in m_world.Martians)
                if (m.IntersectsLine(m_world.Rover.Position, end, 0.5f)) // give martians a wide berth
                {
                    //DrawDebugEllipse(m, Brushes.Yellow);
                    if ((m_world.Rover.Position - m.Position).length() - m.Radius < closest_range)
                    {
                        closest_range = (m_world.Rover.Position - m.Position).length() - m.Radius;
                        closest_obj = m;
                    }
                }

            if (closest_range == float.MaxValue)
            {
                //DrawDebugLine(m_world.Rover.Position, m_world.Rover.Position + (-max_dist * angle), Pens.Blue);
            }

            return closest_range;
        }
    }
}
