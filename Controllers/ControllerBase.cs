using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ICFP08
{
    public class RoverController
    {
        [Flags]
        public enum DebugFlags
        {
            DrawRays = 1,
            DrawProximity = 2,
            DrawDesired = 4,
            DrawCurrent = 8,
            ChooseRandomTarget = 16
        }
        protected DebugFlags m_debugFlags;
        public DebugFlags Flags
        {
            get
            {
                return m_debugFlags;
            }
            set
            {
                m_debugFlags = value;
            }
        }

        protected WorldState m_world = null;
        protected ServerWrapper m_server = null;

        public delegate void LogMessageHandler(string message);
        public event LogMessageHandler LogMessage;

        public delegate void DebugLineHandler(Vector2d start, Vector2d end, Pen pen);
        public event DebugLineHandler DebugLine;
        public delegate void DebugEllipseHandler(MarsObject obj, Brush b);
        public event DebugEllipseHandler DebugEllipse;

        public static string Name
        {
            get
            {
                return "Base";
            }
        }

        public virtual WorldState World
        {
            get
            {
                return m_world;
            }
            set
            {
                m_world = value;
            }
        }
        public virtual ServerWrapper Server
        {
            get
            {
                return m_server;
            }
            set
            {
                m_server = value;
            }
        }
        public virtual Vector2d CurrentTarget
        {
            get
            {
                return new Vector2d();
            }
            set
            {
            }
        }
        public virtual float DesiredHeading
        {
            get
            {
                return 0.0f;
            }
        }

        protected int m_numRayCasts;
        protected int m_numRayTests;
        protected float m_thinkTime;

        public int NumRayCasts
        {
            get
            {
                return m_numRayCasts;
            }
        }

        public int NumRayTests
        {
            get
            {
                return m_numRayTests;
            }
        }

        public float ThinkTime
        {
            get
            {
                return m_thinkTime;
            }
        }

        public RoverController(WorldState world, ServerWrapper server)
        {
            World = world;
            Server = server;
        }

        public virtual void DoUpdate()
        {
            m_numRayTests = 0;
            m_numRayCasts = 0;
        }

        protected void Log(string message)
        {
            if (LogMessage != null)
                LogMessage(message);
        }

        protected void DrawDebugLine(Vector2d start, Vector2d end, Pen pen)
        {
            if (DebugLine != null)
                DebugLine(start, end, pen);
        }

        protected void DrawDebugEllipse(MarsObject obj, Brush b)
        {
            if (DebugEllipse != null)
                DebugEllipse(obj, b);
        }

        protected void DrawDebugRay(Vector2d start, float angle, float length, Pen pen)
        {
            Vector2d vec = new Vector2d(
                (float)Math.Cos(((-1.0f * angle) - 90) * (Math.PI / 180.0f)),
                (float)Math.Sin(((-1.0f * angle) - 90) * (Math.PI / 180.0f)));
            Vector2d end = start + (-length * vec);
            DrawDebugLine(start, end, pen);
        }
    }
}
