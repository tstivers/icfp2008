using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections.ObjectModel;

namespace ICFP08
{
    public class MarsObject
    {
        public MarsObject(Vector2d position, float radius)
        {
            this.m_position = position;
            this.m_radius = radius;
        }

        public override int GetHashCode()
        {
            return m_position.GetHashCode() ^ m_radius.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ((MarsObject)obj).m_position.Equals(this.m_position) &&
                ((MarsObject)obj).m_radius == this.m_radius;
        }

        public override string ToString()
        {
            return "MarsObject(" + m_position.ToString() + ", " + m_radius.ToString() + ")";
        }

        public Vector2d Position
        {
            get
            {
                return m_position;
            }
        }
        public float Radius
        {
            get
            {
                return m_radius;
            }
        }

        public bool IntersectsLine(Vector2d start, Vector2d end, float lineradius)
        {
            Vector2d dir = end - start;
            Vector2d diff = m_position - start;
            float t = diff.dot(dir) / dir.dot(dir);
            t = Math.Max(t, 0.0f);
            t = Math.Min(t, 1.0f);
            Vector2d closest = start + t * dir;
            Vector2d d = m_position - closest;
            float distsqr = d.dot(d);
            return distsqr <= ((m_radius + lineradius) * (m_radius + lineradius));
        }

        protected Vector2d m_position;
        protected float m_radius;
    }

    public class MobileObject : MarsObject
    {
        public MobileObject(Vector2d position, float direction, float speed, float radius) 
            : base(position, radius)
        {
            this.m_direction = direction;
            this.m_speed = speed;
        }

        public float Direction
        {
            get
            {
                return m_direction;
            }
        }
        public float Speed
        {
            get
            {
                return m_speed;
            }
        }

        protected float m_direction;
        protected float m_speed;
    }

    public class Rover : MobileObject
    {
        public Rover(float min_sensor, float max_sensor, float max_turn, float max_hard_turn) 
            : base(new Vector2d(), 0.0f, 0.0f, 0.5f)
        {
            this.m_min_sensor = min_sensor;
            this.m_max_sensor = max_sensor;
            this.m_max_turn = max_turn;
            this.m_max_hard_turn = max_hard_turn;
            this.m_move_state = MoveType.Roll;
            this.m_turn_state = TurnType.Straight;
        }

        public void UpdateState(Vector2d position, float direction, float speed, MoveType move_state, TurnType turn_state)
        {
            this.m_position = position;
            this.m_direction = direction;
            this.m_speed = speed;
            this.m_move_state = move_state;
            this.m_turn_state = turn_state;
        }

        public MoveType MoveState
        {
            get
            {
                return m_move_state;
            }
        }

        public TurnType TurnState
        {
            get
            {
                return m_turn_state;
            }
        }

        // degrees per second
        public float TurnSpeed
        {
            get
            {
                return m_max_turn;
            }
        }
        public float HardTurnSpeed
        {
            get
            {
                return m_max_hard_turn;
            }
        }

        protected float m_min_sensor;
        protected float m_max_sensor;
        protected float m_max_turn;
        protected float m_max_hard_turn;
        protected MoveType m_move_state;
        protected TurnType m_turn_state;
    }

    public class Martian : MobileObject
    {
        public Martian(Vector2d position, float direction, float speed) 
            : base(position, direction, speed, 2.5f) // oversize martians because they're scary BUT DON'T OVERDO IT
        {
        }
    }

    public class Crater : MarsObject
    {
        public Crater(Vector2d position, float radius)
            : base(position, radius)
        {
        }
    }

    public class Boulder : MarsObject
    {
        public Boulder(Vector2d position, float radius)
            : base(position, radius)
        {
        }
    }

    public class Home : MarsObject
    {
        public Home(Vector2d position, float radius)
            : base(position, radius)
        {
        }
    }

    public class WorldState
    {
        private Dictionary<Boulder, int> m_boulders = new Dictionary<Boulder,int>();
        private Dictionary<Crater, int> m_craters = new Dictionary<Crater, int>();
        private List<Martian> m_martians = new List<Martian>();
        private Rover m_rover = null;
        private Home m_home = null;
        private SizeF m_size;
        private int m_timeLimit = 0;
        private int m_lastUpdate = 0;

        public SizeF Size
        {
            get
            {
                return m_size;
            }
        }
        public int TimeLimit
        {
            get
            {
                return m_timeLimit;
            }
        }
        public ReadOnlyCollection<Boulder> Boulders
        {
            get
            {
                Boulder[] boulders = new Boulder[m_boulders.Keys.Count];
                m_boulders.Keys.CopyTo(boulders, 0);
                return new ReadOnlyCollection<Boulder>(boulders);
            }
        }
        public ReadOnlyCollection<Crater> Craters
        {
            get
            {
                Crater[] craters = new Crater[m_craters.Keys.Count];
                m_craters.Keys.CopyTo(craters, 0);
                return new ReadOnlyCollection<Crater>(craters);
            }
        }
        public ReadOnlyCollection<Martian> Martians
        {
            get
            {
                return m_martians.AsReadOnly();
            }
        }
        public Rover Rover
        {
            get
            {
                return m_rover;
            }
        }
        public Home Home
        {
            get
            {
                return m_home;
            }
        }
        public bool FoundHome
        {
            get
            {
                return Home != null;
            }
        }

        public delegate void ObjectHandler(WorldState world, MarsObject obj);
        public delegate void BoulderHandler(WorldState world, Boulder b);
        public delegate void CraterHandler(WorldState world, Crater c);
        public delegate void HomeHandler(WorldState world, Home h);
        public delegate void MartianHandler(WorldState world, Martian m);
        public delegate void WorldHandler(WorldState world);

        public event ObjectHandler ObjectFound;
        public event ObjectHandler ObjectSeen;
        public event BoulderHandler BoulderFound;
        public event BoulderHandler BoulderSeen;
        public event CraterHandler CraterFound;
        public event CraterHandler CraterSeen;
        public event MartianHandler MartianSeen;
        public event HomeHandler HomeFound;
        public event HomeHandler HomeSeen;
        public event WorldHandler WorldChanged;

        public WorldState(InitializationMessage init)
        {            
            m_size = init.size;
            m_rover = new Rover(init.min_sensor, init.max_sensor, init.max_turn, init.max_hard_turn);
            m_timeLimit = init.time_limit;
        }

        public void UpdateWorldState(TelemetryMessage telemetry)
        {
            m_lastUpdate = telemetry.time_stamp;
            m_rover.UpdateState(telemetry.position, telemetry.direction, telemetry.speed, telemetry.move_state, telemetry.turn_state);
            foreach (ObstacleMessage obstacle in telemetry.obstacles)
            {
                switch (obstacle.type)
                {
                    case ObstacleType.Boulder:
                        Boulder b = new Boulder(obstacle.position, obstacle.radius);
                        if (!m_boulders.ContainsKey(b))
                        {
                            m_boulders.Add(b, telemetry.time_stamp);
                            OnBoulderFound(b);
                        }
                        else
                        {
                            m_boulders[b] = telemetry.time_stamp;
                            OnBoulderSeen(b);
                        }
                        break;
                    case ObstacleType.Crater:
                        Crater c = new Crater(obstacle.position, obstacle.radius);                        
                        if (!m_craters.ContainsKey(c))
                        {
                            m_craters.Add(c, telemetry.time_stamp);
                            OnCraterFound(c);
                        }
                        else
                        {
                            m_craters[c] = telemetry.time_stamp;
                            OnCraterSeen(c);
                        }
                        break;
                    case ObstacleType.Home:
                        if (m_home == null)
                        {
                            m_home = new Home(obstacle.position, obstacle.radius);
                            OnHomeFound(m_home);
                        }
                        else
                        {
                            OnHomeSeen(m_home);
                        }
                        break;
                }
            }
            m_martians.Clear();
            foreach (MartianMessage m in telemetry.martians)
            {
                Martian martian = new Martian(m.position, m.direction, m.speed);
                m_martians.Add(martian);
                OnMartianSeen(martian);
            }
            OnWorldChanged();
        }

        public void ForgetStuff()
        {
            m_boulders.Clear();
            m_craters.Clear();
        }

        private void OnWorldChanged()
        {
            if (WorldChanged != null)
                WorldChanged(this);
        }

        private void OnMartianSeen(Martian m)
        {
            OnObjectSeen(m);
            if (MartianSeen != null)
                MartianSeen(this, m);
        }

        private void OnHomeSeen(Home h)
        {
            OnObjectSeen(h);
            if (HomeSeen != null)
                HomeSeen(this, h);
        }

        private void OnHomeFound(Home h)
        {
            OnObjectFound(h);
            if (HomeFound != null)
                HomeFound(this, h);
        }

        private void OnCraterSeen(Crater c)
        {
            OnObjectSeen(c);
            if (CraterSeen != null)
                CraterSeen(this, c);
        }

        private void OnCraterFound(Crater c)
        {
            OnObjectFound(c);
            if (CraterFound != null)
                CraterFound(this, c);
        }

        private void OnBoulderSeen(Boulder b)
        {
            OnObjectSeen(b);
            if (BoulderSeen != null)
                BoulderSeen(this, b);
        }

        private void OnBoulderFound(Boulder b)
        {
            OnObjectFound(b);
            if (BoulderFound != null)
                BoulderFound(this, b);
        }

        private void OnObjectSeen(MarsObject o)
        {
            if (ObjectSeen != null)
                ObjectSeen(this, o);
        }

        private void OnObjectFound(MarsObject o)
        {
            if (ObjectFound != null)
                ObjectFound(this, o);
        }

        public void ResetWorldState()
        {
            m_rover.UpdateState(new Vector2d(), 0, 0, MoveType.Roll, TurnType.Straight);
        }
    }
}
