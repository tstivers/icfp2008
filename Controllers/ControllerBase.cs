using System;
using System.Collections.Generic;
using System.Text;

namespace ICFP08
{
    public class RoverController
    {
        protected WorldState m_world = null;
        protected ServerWrapper m_server = null;

        public delegate void LogMessageHandler(string message);
        public event LogMessageHandler LogMessage;

        public delegate void DebugLineHandler(Vector2d start, Vector2d end);
        public event DebugLineHandler DebugLine;

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

        protected void DrawDebugLine(Vector2d start, Vector2d end)
        {
            if (DebugLine != null)
                DebugLine(start, end);
        }
    }
}
