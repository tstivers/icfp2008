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
        private float m_minAngle = 0.1f;
        private float m_fastAngle = 15.0f;
        private int m_maxTries = 360; // number of times to try and find a non-colliding heading
        private float m_spiralSpeed = 3.0f;
        private float m_avoidAngle = 1.0f;
        private float m_brakeAngle = 65.0f; // brake if trying to turn more than xx degrees
        private float m_brakeSpeed = 3.0f; // min speed for braking
        private TurnType m_pendingTurn = TurnType.Straight;
        private MoveType m_pendingThrottle = MoveType.Roll;
        private bool m_inSpiral = false;

        private float m_shortTurnAngle = 3.0f;
        private double m_msPerDegree = 25.0;
        private double m_msPadding = 10.0;

        private struct MoveTableEntry
        {
            public readonly MoveType start;
            public readonly MoveType end;
            public MoveTableEntry(MoveType start, MoveType end)
            {
                this.start = start;
                this.end = end;
            }

            public override bool Equals(object obj)
            {
                return this.start == ((MoveTableEntry)obj).start &&
                    this.end == ((MoveTableEntry)obj).end;
            }
            public override int GetHashCode()
            {
                return start.GetHashCode() ^ end.GetHashCode();
            }
        }
        private struct TurnTableEntry
        {
            public readonly TurnType start;
            public readonly TurnType end;
            public TurnTableEntry(TurnType start, TurnType end)
            {
                this.start = start;
                this.end = end;
            }

            public override bool Equals(object obj)
            {
                return this.start == ((TurnTableEntry)obj).start &&
                    this.end == ((TurnTableEntry)obj).end;
            }
            public override int GetHashCode()
            {
                return start.GetHashCode() ^ end.GetHashCode();
            }
        }

        private Dictionary<MoveTableEntry, int> m_moveTable = new Dictionary<MoveTableEntry, int>();
        private Dictionary<TurnTableEntry, int> m_turnTable = new Dictionary<TurnTableEntry, int>();

        private void PopulateMoveTable()
        {
            m_moveTable.Add(new MoveTableEntry(MoveType.Accelerate, MoveType.Accelerate), 0);
            m_moveTable.Add(new MoveTableEntry(MoveType.Accelerate, MoveType.Roll), -1);
            m_moveTable.Add(new MoveTableEntry(MoveType.Accelerate, MoveType.Brake), -2);
            m_moveTable.Add(new MoveTableEntry(MoveType.Roll, MoveType.Accelerate), 1);
            m_moveTable.Add(new MoveTableEntry(MoveType.Roll, MoveType.Roll), 0);
            m_moveTable.Add(new MoveTableEntry(MoveType.Roll, MoveType.Brake), -1);
            m_moveTable.Add(new MoveTableEntry(MoveType.Brake, MoveType.Accelerate), 2);
            m_moveTable.Add(new MoveTableEntry(MoveType.Brake, MoveType.Roll), 1);
            m_moveTable.Add(new MoveTableEntry(MoveType.Brake, MoveType.Brake), 0);
        }
        private void PopulateTurnTable()
        {
            m_turnTable.Add(new TurnTableEntry(TurnType.HardLeft, TurnType.HardLeft),   0);
            m_turnTable.Add(new TurnTableEntry(TurnType.HardLeft, TurnType.Left),       1);
            m_turnTable.Add(new TurnTableEntry(TurnType.HardLeft, TurnType.Straight),   2);
            m_turnTable.Add(new TurnTableEntry(TurnType.HardLeft, TurnType.Right),      3);
            m_turnTable.Add(new TurnTableEntry(TurnType.HardLeft, TurnType.HardRight),  4);

            m_turnTable.Add(new TurnTableEntry(TurnType.Left, TurnType.HardLeft),       -1);
            m_turnTable.Add(new TurnTableEntry(TurnType.Left, TurnType.Left),           0);
            m_turnTable.Add(new TurnTableEntry(TurnType.Left, TurnType.Straight),       1);
            m_turnTable.Add(new TurnTableEntry(TurnType.Left, TurnType.Right),          2);
            m_turnTable.Add(new TurnTableEntry(TurnType.Left, TurnType.HardRight),      3);

            m_turnTable.Add(new TurnTableEntry(TurnType.Straight, TurnType.HardLeft),   -2);
            m_turnTable.Add(new TurnTableEntry(TurnType.Straight, TurnType.Left),       -1);
            m_turnTable.Add(new TurnTableEntry(TurnType.Straight, TurnType.Straight),   0);
            m_turnTable.Add(new TurnTableEntry(TurnType.Straight, TurnType.Right),      1);
            m_turnTable.Add(new TurnTableEntry(TurnType.Straight, TurnType.HardRight),  2);

            m_turnTable.Add(new TurnTableEntry(TurnType.Right, TurnType.HardLeft),      -3);
            m_turnTable.Add(new TurnTableEntry(TurnType.Right, TurnType.Left),          -2);
            m_turnTable.Add(new TurnTableEntry(TurnType.Right, TurnType.Straight),      -1);
            m_turnTable.Add(new TurnTableEntry(TurnType.Right, TurnType.Right),         0);
            m_turnTable.Add(new TurnTableEntry(TurnType.Right, TurnType.HardRight),     1);

            m_turnTable.Add(new TurnTableEntry(TurnType.HardRight, TurnType.HardLeft),  -4);
            m_turnTable.Add(new TurnTableEntry(TurnType.HardRight, TurnType.Left),      -3);
            m_turnTable.Add(new TurnTableEntry(TurnType.HardRight, TurnType.Straight),  -2);
            m_turnTable.Add(new TurnTableEntry(TurnType.HardRight, TurnType.Right),     -1);
            m_turnTable.Add(new TurnTableEntry(TurnType.HardRight, TurnType.HardRight), 0);

        }

        public int StepsBetween(TurnType current, TurnType wanted)
        {
            return m_turnTable[new TurnTableEntry(current, wanted)];
        }

        public int StepsBetween(MoveType current, MoveType wanted)
        {
            return m_moveTable[new MoveTableEntry(current, wanted)];
        }

        public void DoShortTurn(TurnType direction, double time)
        {
            DoTurn(direction);
            Stopwatch s = Stopwatch.StartNew();
            while (s.Elapsed.TotalMilliseconds < time + m_msPadding) ; // spin
            DoTurn(TurnType.Straight);
            m_pendingTurn = TurnType.Straight;
        }

        public void DoTurn(TurnType direction)
        {
            int steps = StepsBetween(m_pendingTurn, direction);
            string command = "";
            if (steps > 0) // turning right
            {
                for (int i = 0; i < steps; i++)
                    command += "r;";
                m_server.SendMessage(command);
            }
            else if (steps < 0)
            {
                for (int i = 0; i < Math.Abs(steps); i++)
                    command += "l;";
                m_server.SendMessage(command);
            }
            m_pendingTurn = direction;
        }

        public void SetThrottle(MoveType throttle)
        {
            int steps = StepsBetween(m_pendingThrottle, throttle);
            string command = "";
            if (steps > 0) // accelerating
            {
                for (int i = 0; i < steps; i++)
                    command += "a;";
                m_server.SendMessage(command);
            }
            else if (steps < 0)
            {
                for (int i = 0; i < Math.Abs(steps); i++)
                    command += "b;";
                m_server.SendMessage(command);
            }
            m_pendingThrottle = throttle;
        }

        public void TurnTo(float heading)
        {
            float turn_angle = heading - m_world.Rover.Direction;
            if (turn_angle > 180.0f) turn_angle = -360.0f + turn_angle;
            if (turn_angle < -180.0f) turn_angle = 360.0f - turn_angle;

            if (Math.Abs(turn_angle) < m_minAngle) // don't turn
                return;

            TurnType direction;
            if (Math.Abs(turn_angle) > m_fastAngle) // hard turn
            {
                direction = turn_angle > 0.0 ? TurnType.HardRight : TurnType.HardLeft;
                DoTurn(direction);
            }
            else if (Math.Abs(turn_angle) > m_shortTurnAngle) // normal turn
            {
                direction = turn_angle > 0.0 ? TurnType.Right : TurnType.Left;
                DoTurn(direction);
            } 
            else if (Math.Abs(turn_angle) > m_minAngle) // short turn
            {
                direction = turn_angle > 0.0 ? TurnType.Right : TurnType.Left;
                float turn_time = (float)(Math.Abs(turn_angle) * m_msPerDegree);
                DoShortTurn(direction, turn_time);
            }
            else // go straight
                DoTurn(TurnType.Straight);
        }

        public override Vector2d CurrentTarget
        {
            get
            {
                return m_target;
            }
            set
            {
                m_target = value;
                m_inSpiral = false; // reset death spiral
                Log("New Target: " + value);
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
            PopulateMoveTable();
            PopulateTurnTable();
        }

        void m_server_EndOfRunMessage(object sender, EndOfRunMessageEventArgs ee)
        {
            m_pendingTurn = TurnType.Straight;
            m_pendingThrottle = MoveType.Roll;
            m_inSpiral = false;
            m_target = new Vector2d();
        }

        public override void DoUpdate()
        {
            base.DoUpdate();

            // calculate our new desired heading
            if ((m_debugFlags & DebugFlags.ChooseRandomTarget) == DebugFlags.ChooseRandomTarget)
                m_target = ChooseRandomTarget();
            else
                m_target = ChooseTarget();
            Vector2d offset = m_target - m_world.Rover.Position;
            m_desiredHeading = -(float)(Math.Atan2(offset.y, offset.x) * (180.0f / Math.PI)) + 90.0f;
            float target_distance = (m_world.Rover.Position - m_target).length();
            if (m_desiredHeading < 0.0f)
                m_desiredHeading += 360.0f;
            if (m_desiredHeading > 360.0f)
                m_desiredHeading -= 360.0f;

            // check for collisions on our current desired trajectory
            if (TrajectoryHitsObject(m_desiredHeading, target_distance, 1.0f, true))
            {
               // find a new heading
                for (int num_tries = 1; num_tries < m_maxTries; num_tries++)
                {
                    float angle_offset = (float)num_tries * m_avoidAngle;

                    if (!TrajectoryHitsObject(m_desiredHeading + angle_offset, target_distance, 1.0f, true)) // check right
                    {
                        m_desiredHeading += angle_offset;
                        break;
                    }

                    if (!TrajectoryHitsObject(m_desiredHeading - angle_offset, target_distance, 1.0f, true)) // check left
                    {
                        m_desiredHeading -= angle_offset;
                        break;
                    }
                }
            }

            //DrawDebugRay(m_world.Rover.Position, m_desiredHeading, max_distance, Pens.Green);

            // execute any required turn            
            float turn_angle = m_desiredHeading - m_world.Rover.Direction;
            if (turn_angle > 180.0f)
                turn_angle = -360.0f + turn_angle;
            if (turn_angle < -180.0f)
                turn_angle = 360.0f - turn_angle;
            TurnTo(m_desiredHeading);

            MoveType gas = MoveType.Accelerate;  // default to accellerate

            // proximity alarm
            if (TrajectoryHitsObject(m_world.Rover.Direction, m_world.Rover.Speed * 1.5f, 0.5f, false))
            {
                Log("WHOOP WHOOP PULL UP");
                gas = MoveType.Brake;
                if ((m_debugFlags & DebugFlags.DrawProximity) == DebugFlags.DrawProximity)
                    DrawDebugRay(m_world.Rover.Position, m_world.Rover.Direction, m_world.Rover.Speed * 1.5f, Pens.Red);
            }
            else
                if ((m_debugFlags & DebugFlags.DrawProximity) == DebugFlags.DrawProximity)
                    DrawDebugRay(m_world.Rover.Position, m_world.Rover.Direction, m_world.Rover.Speed * 1.5f, Pens.Green);

            // avoid the SPIRAL OF DEATH
            if (offset.length() < (m_world.Rover.Speed * 2.0f) &&
                (m_pendingTurn == TurnType.HardLeft || m_pendingTurn == TurnType.HardRight))
            {
                Log("IN SPIRAL OF DEATH");
                m_inSpiral = true;
            }

            if (m_inSpiral && m_world.Rover.Speed > m_spiralSpeed)
                gas = MoveType.Brake;

            // brake if turning more than m_brakeAngle degrees
            if (gas == MoveType.Accelerate &&
                Math.Abs(turn_angle) > m_brakeAngle &&
                m_world.Rover.Speed > m_brakeSpeed)
            {
                //Log("BRAKING");
                gas = MoveType.Brake;
            }

            // set our speed
            SetThrottle(gas);
        }

        private Vector2d ChooseTarget()
        {
            if (m_world.FoundHome)
                return m_world.Home.Position;
            return new Vector2d(0, 0);
        }

        static Random m_random = new Random();
        private Vector2d ChooseRandomTarget()
        {
            if((m_target - m_world.Rover.Position).length() < 10.0f || (m_target.x == 0.0 && m_target.y == 0.0))
            {
                // pick a new random target
                Vector2d target = new Vector2d(
                    (float)((m_random.NextDouble() * m_world.Size.Width) - (m_world.Size.Width / 2.0)),
                    (float)((m_random.NextDouble() * m_world.Size.Height) - (m_world.Size.Height / 2.0)));
                Log("New Target: " + target);
                m_inSpiral = false; // reset death spiral
                return target;
            }
            return m_target;
        }

        private bool TrajectoryHitsObject(float heading, float max_dist, float padding, bool check_martians)
        {
            MarsObject dummy = null;
            return float.MaxValue != NearestObject(heading, max_dist, padding, ref dummy, check_martians);
        }

        private float NearestObject(float heading, float max_dist, float padding, ref MarsObject closest_obj, bool check_martians)
        {
            m_numRayCasts++;

            Vector2d angle = new Vector2d(
                (float)Math.Cos(((-1.0f * heading) - 90) * (Math.PI / 180.0f)), 
                (float)Math.Sin(((-1.0f * heading) - 90) * (Math.PI / 180.0f)));
            Vector2d end = m_world.Rover.Position + (-max_dist * angle);
            //DrawDebugLine(m_world.Rover.Position, end, Pens.Red);

            float closest_range = float.MaxValue;

            foreach (Boulder b in m_world.Boulders)
            {
                m_numRayTests++;
                if (b.IntersectsLine(m_world.Rover.Position, end, padding))
                {
                    //DrawDebugEllipse(b, Brushes.Yellow);
                    float range = (m_world.Rover.Position - b.Position).length() - (b.Radius + padding);
                    if (range < closest_range && range > 0.0f) // ignore the obstacle if we think we are inside of it (avoids panics)
                    {
                        closest_range = range;
                        closest_obj = b;
                    }
                }
            }

            foreach (Crater c in m_world.Craters)
            {
                m_numRayTests++;
                if (c.IntersectsLine(m_world.Rover.Position, end, padding))
                {
                    //DrawDebugEllipse(c, Brushes.Yellow);
                    float range = (m_world.Rover.Position - c.Position).length() - (c.Radius + padding);
                    if (range < closest_range && range > 0.0f) // ignore the obstacle if we think we are inside of it (avoids panics)
                    {
                        closest_range = range;
                        closest_obj = c;
                    }
                }
            }

            if (check_martians)
            {
                foreach (Martian m in m_world.Martians)
                {
                    m_numRayTests++;
                    if (m.IntersectsLine(m_world.Rover.Position, end, 5.0f)) // give martians a wide berth
                    {
                        //DrawDebugEllipse(m, Brushes.Yellow);
                        float range = (m_world.Rover.Position - m.Position).length() - (m.Radius + padding);
                        if (range < closest_range && range > 0.0f) // ignore the obstacle if we think we are inside of it (avoids panics)
                        {
                            closest_range = range;
                            closest_obj = m;
                        }
                    }
                }
            }

            if (closest_range == float.MaxValue)
            {
                if((m_debugFlags & DebugFlags.DrawRays) == DebugFlags.DrawRays)
                    DrawDebugLine(m_world.Rover.Position, end, Pens.Blue);
            }
            else if ((m_debugFlags & DebugFlags.DrawRays) == DebugFlags.DrawRays)
                DrawDebugLine(m_world.Rover.Position, end, Pens.Red);

            return closest_range;
        }
    }
}
