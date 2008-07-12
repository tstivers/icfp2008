using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ICFP08
{
    public enum OBJECT_TYPE
    {
        BOULDER,
        CRATER,
        HOME
    }

    public class ObjectPos
    {
        public ObjectPos(float x, float y, float r)
        {
            m_pos = new PointF(x, y);
            m_radius = r;
        }

        public override int GetHashCode()
        {
            return m_pos.X.GetHashCode() ^ m_pos.Y.GetHashCode() ^ m_radius.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            ObjectPos other = (ObjectPos)obj;
            return (other.Pos.X == this.Pos.X) && (other.Pos.Y == this.Pos.Y) && (other.Radius == this.Radius);
        }

        public override string ToString()
        {
            return "(" + m_pos.X + ", " + m_pos.Y + ", " + m_radius + ")";
        }

        private PointF m_pos;
        private float m_radius;

        public PointF Pos
        {
            get
            {
                return m_pos;
            }
        }

        public float Radius
        {
            get
            {
                return m_radius;
            }
        }
    }

    public class ObjectDiscoveredEventArgs : EventArgs
    {
        public ObjectDiscoveredEventArgs(ObjectPos pos, OBJECT_TYPE type)
        {
            this.position = pos;
            this.type = type;
        }

        public readonly ObjectPos position;
        public readonly OBJECT_TYPE type;
    }

    public class StaticObjects
    {
        public readonly SizeF Size;
        private PointF m_homePos;
        private float m_homeRadius;
        private bool m_homeFound = false;
        private Dictionary<ObjectPos, OBJECT_TYPE> m_objects = new Dictionary<ObjectPos,OBJECT_TYPE>();

        public delegate void ObjectDiscoveredEventHandler(object sender, ObjectDiscoveredEventArgs ode);
        public event ObjectDiscoveredEventHandler ObjectDiscovered;

        public bool HomeFound
        {
            get
            {
                return m_homeFound;
            }
        }

        public PointF HomePos
        {
            get
            {
                return m_homePos;
            }
        }

        public float HomeRadius
        {
            get
            {
                return m_homeRadius;
            }
        }

        public StaticObjects(float xsize, float ysize)
        {
            Size = new SizeF(xsize, ysize);
        }

        public void AddCrater(float xpos, float ypos, float radius)
        {
            ObjectPos pos = new ObjectPos(xpos, ypos, radius);
            if (!m_objects.ContainsKey(pos))
            {
                m_objects.Add(pos, OBJECT_TYPE.CRATER);
                if (ObjectDiscovered != null)
                    ObjectDiscovered(this, new ObjectDiscoveredEventArgs(pos, OBJECT_TYPE.CRATER));
            }
        }

        public void AddBoulder(float xpos, float ypos, float radius)
        {
            ObjectPos pos = new ObjectPos(xpos, ypos, radius);
            if (!m_objects.ContainsKey(pos))
            {
                m_objects.Add(pos, OBJECT_TYPE.BOULDER);
                if (ObjectDiscovered != null)
                    ObjectDiscovered(this, new ObjectDiscoveredEventArgs(pos, OBJECT_TYPE.BOULDER));
            }
        }

        public void AddHome(float xpos, float ypos, float radius)
        {
            if (m_homeFound)
                return;
            
            ObjectPos pos = new ObjectPos(xpos, ypos, radius);
            m_objects.Add(pos, OBJECT_TYPE.HOME);
            if (ObjectDiscovered != null)
                ObjectDiscovered(this, new ObjectDiscoveredEventArgs(pos, OBJECT_TYPE.HOME));

            m_homePos = new PointF(xpos, ypos);
            m_homeRadius = radius;
            m_homeFound = true;
        }
    }
}
