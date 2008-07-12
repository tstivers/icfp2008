using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ICFP08
{
    public class MarsObject
    {
        public MarsObject(Vector2d position, float radius)
        {
            this.position = position;
            this.radius = radius;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode() ^ radius.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ((MarsObject)obj).position.Equals(this.position) &&
                ((MarsObject)obj).radius == this.radius;
        }

        public override string ToString()
        {
            return "MarsObject(" + position.ToString() + ", " + radius.ToString() + ")";
        }

        public readonly Vector2d position;
        public readonly float radius;
    }

    public class MobileObject : MarsObject
    {
        public MobileObject(Vector2d position, float direction, float speed, float radius) : base(position, radius)
        {
            this.direction = direction;
            this.speed = speed;
        }

        float direction;
        float speed;
    }

    public class Rover : MobileObject
    {
        public Rover() : base(new Vector2d(), 0.0f, 0.0f, 5.0f)
        {
        }
    }

    public class Martian : MobileObject
    {
        public Martian(Vector2d position, float direction, float speed) : base(position, direction, speed, 2.5f)
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

    class WorldState
    {
        private List<Boulder> m_boulders;
        private List<Crater> m_craters;
        private List<Martian> m_martians;
        private Rover m_rover;
        private Home m_home;
        private SizeF m_size;

        public SizeF Size
        {
            get
            {
                return m_size;
            }
        }

        public WorldState(float width, float height)
        {
            m_size = new SizeF(width, height);
        }
    }
}
