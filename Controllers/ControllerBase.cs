using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ICFP08
{
    public class RoverController
    {
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
        }
        public virtual float DesiredHeading
        {
            get
            {
                return 0.0f;
            }
        }

        public RoverController(WorldState world, ServerWrapper server)
        {
            World = world;
            Server = server;
        }

        public virtual void DoUpdate()
        {
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
