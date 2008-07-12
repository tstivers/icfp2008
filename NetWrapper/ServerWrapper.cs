using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace ICFP08
{

    #region ENUMS
    public enum MOVE_STATE
    {
        accelerating,
        braking,
        rolling
    }
    public enum TURN_STATE
    {
        hard_left,
        left,
        straight,
        right,
        hard_right
    }
    public enum OBSTACLE_KIND
    {
        boulder,
        crater,
        home
    }
    #endregion ENUMS

    #region EventArgs
    public class InitializationMessageEventArgs : EventArgs
    {
        public InitializationMessageEventArgs(float dx, float dy, int time_limit, float min_sensor, float max_sensor, float max_speed, float max_turn, float max_hard_turn)
        {
            this.dx = dx;
            this.dy = dy;
            this.time_limit = time_limit;
            this.min_sensor = min_sensor;
            this.max_sensor = max_sensor;
            this.max_speed = max_speed;
            this.max_turn = max_turn;
            this.max_hard_turn = max_hard_turn;
        }

        public readonly float dx;
        public readonly float dy;
        public readonly int time_limit;
        public readonly float min_sensor;
        public readonly float max_sensor;
        public readonly float max_speed;
        public readonly float max_turn;
        public readonly float max_hard_turn;
    }
    public class TelemetryMessageEventArgs : EventArgs
    {
        public class ObstacleInfo
        {
            public ObstacleInfo(OBSTACLE_KIND kind, float xpos, float ypos, float radius)
            {
                this.kind = kind;
                this.xpos = xpos;
                this.ypos = ypos;
                this.radius = radius;
            }

            public readonly OBSTACLE_KIND kind;
            public readonly float xpos;
            public readonly float ypos;
            public readonly float radius;
        }

        public class MartianInfo
        {
            public MartianInfo(float xpos, float ypos, float direction, float speed)
            {
                this.xpos = xpos;
                this.ypos = ypos;
                this.direction = direction;
                this.speed = speed;
            }

            public readonly float xpos;
            public readonly float ypos;
            public readonly float direction;
            public readonly float speed;
        }

        public TelemetryMessageEventArgs(int time_stamp, MOVE_STATE move_state, TURN_STATE turn_state, float xpos, float ypos, float direction,
            float speed,
            ObstacleInfo[] obstacles,
            MartianInfo[] martians)
        {
            this.time_stamp = time_stamp;
            this.move_state = move_state;
            this.turn_state = turn_state;
            this.xpos = xpos;
            this.ypos = ypos;
            this.direction = direction;
            this.speed = speed;
            this.obstacles = obstacles;
            this.martians = martians;
        }

        public readonly int time_stamp;
        public readonly MOVE_STATE move_state;
        public readonly TURN_STATE turn_state;
        public readonly float xpos;
        public readonly float ypos;
        public readonly float direction;
        public readonly float speed;
        public readonly ObstacleInfo[] obstacles;
        public readonly MartianInfo[] martians;
    }
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(string message)
        {
            this.message = message;
        }

        public readonly string message;
    }
    public class EventMessageEventArgs : EventArgs
    {
        public EventMessageEventArgs(int time_stamp)
        {
            this.time_stamp = time_stamp;
        }

        public readonly int time_stamp;
    }
    public class EndOfRunMessageEventArgs : EventArgs
    {
        public EndOfRunMessageEventArgs(int time_stamp, int score)
        {
            this.time_stamp = time_stamp;
            this.score = score;
        }

        public readonly int score;
        public readonly int time_stamp;
    }
    #endregion EventArgs

    /// <summary>
    ///  from msdn
    /// </summary>
    public class SocketState
    {
        public Socket workSocket = null;
        public const int BUFFER_SIZE = 1024;
        public byte[] buffer = new byte[BUFFER_SIZE];
        public StringBuilder sb = new StringBuilder();
        public ServerWrapper wrapper = null;
    }

    public class Message
    {
        public Message(string message)
        {
            this.message = message;
        }
        public string message;
    }

    public class ServerWrapper
    {
        public delegate void MessageReceievedEventHandler(object sender, MessageEventArgs me);
        public event MessageReceievedEventHandler MessageReceived;

        public delegate void InitializationMessageEventHandler(object sender, InitializationMessageEventArgs ime);
        public event InitializationMessageEventHandler InitializationMessage;

        public delegate void TelemetryMessageEventHandler(object sender, TelemetryMessageEventArgs tme);
        public event TelemetryMessageEventHandler TelemetryMessage;

        public delegate void EventMessageEventHandler(object sender, EventMessageEventArgs ae);
        public event EventMessageEventHandler CrashMessage;
        public event EventMessageEventHandler CraterMessage;
        public event EventMessageEventHandler KilledMessage;
        public event EventMessageEventHandler SuccessMessage;

        public delegate void EndOfRunMessageEventHandler(object sender, EndOfRunMessageEventArgs ee);
        public event EndOfRunMessageEventHandler EndOfRunMessage;

        protected Socket m_socket;
        /// <summary>
        /// protects the message list
        /// </summary>
        protected Object m_lock = new Object();
        public List<Message> m_messages = new List<Message>();

        public bool Connected
        {
            get
            {
                if (m_socket != null)
                    return m_socket.Connected;
                else
                    return false;
            }
        }

        /// <summary>
        /// taken from msdn
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private static Socket ConnectSocket(string server, int port)
        {
            Socket s = null;
            IPHostEntry hostEntry = null;

            // Get host related information.
            hostEntry = Dns.GetHostEntry(server);

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host IP Address is not compatible with the address family
            // (typical in the IPv6 case).
            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket =
                    new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    s = tempSocket;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return s;
        }

        public void Connect(string host, int port)
        {
            if (Connected)
                Disconnect();

            m_socket = ConnectSocket(host, port);
            m_socket.NoDelay = true; // DISABLE NAGLE
            
            // start receiving data
            ReceiveMessage();
        }

        private void Disconnect()
        {
            m_socket.Disconnect(false);
        }

        public void ProcessMessages()
        {
            lock(m_lock)
            {
                foreach(Message m in m_messages)
                {
                    if (MessageReceived != null)
                        MessageReceived(this, new MessageEventArgs(m.message));
                    ParseMessage(m.message);
                }
                m_messages.Clear();
            }
        }

        public void SendCommand(MOVE_STATE ms, TURN_STATE ts)
        {
            if (!Connected)
                return;

            string command = "";
            if(ms == MOVE_STATE.accelerating)
                command += "a";
            else if(ms == MOVE_STATE.braking)
                command += "b";
            if(ts == TURN_STATE.left || ts == TURN_STATE.hard_left)
                command += "l";
            else if(ts == TURN_STATE.right || ts == TURN_STATE.hard_right)
                command += "r";
            command += ";";
            byte[] buffer = Encoding.ASCII.GetBytes(command);
            m_socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Write_Callback), this);
        }

        private void ParseMessage(string p)
        {
            string[] tokens = p.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            switch (tokens[0])
            {
                case "I":
                    ParseInitialization(tokens);
                    break;
                case "T":
                    ParseTelemetry(tokens);
                    break;
                case "B":
                    if (CrashMessage != null)
                        CrashMessage(this, new EventMessageEventArgs(int.Parse(tokens[1])));
                    break;
                case "C":
                    if (CraterMessage != null)
                        CraterMessage(this, new EventMessageEventArgs(int.Parse(tokens[1])));
                    break;
                case "K":
                    if (KilledMessage != null)
                        KilledMessage(this, new EventMessageEventArgs(int.Parse(tokens[1])));
                    break;
                case "S":
                    if (SuccessMessage != null)
                        SuccessMessage(this, new EventMessageEventArgs(int.Parse(tokens[1])));
                    break;
                case "E":
                    if (EndOfRunMessage != null)
                        EndOfRunMessage(this, new EndOfRunMessageEventArgs(int.Parse(tokens[1]), int.Parse(tokens[2])));
                    break;
                default:
                    break;
            }
        }

        private void ParseTelemetry(string[] argv)
        {
            int time = int.Parse(argv[1]);
            MOVE_STATE ms = MOVE_STATE.rolling;
            switch(argv[2][0])
            {
                case 'a':
                    ms = MOVE_STATE.accelerating;
                    break;
                case 'b':
                    ms = MOVE_STATE.braking;
                    break;
                case '-':
                    ms = MOVE_STATE.rolling;
                    break;
            }

            TURN_STATE ts = TURN_STATE.straight;
            switch(argv[2][1])
            {
                case 'L':
                    ts = TURN_STATE.hard_left;
                    break;
                case 'l':
                    ts = TURN_STATE.left;
                    break;
                case '-':
                    ts = TURN_STATE.straight;
                    break;
                case 'r':
                    ts = TURN_STATE.right;
                    break;
                case 'R':
                    ts = TURN_STATE.hard_right;
                    break;
            }

            float xpos = float.Parse(argv[3]);
            float ypos = float.Parse(argv[4]);
            float direction = -float.Parse(argv[5]) + 90;
            if (direction < 0.0f)
                direction += 360.0f;
            float speed = float.Parse(argv[6]);

            int argc = 7;
            int obstacle_count = 0;
            int martian_count = 0;
            while (argc < argv.Length)
            {
                switch (argv[argc][0])
                {
                    case 'b':
                    case 'c':
                    case 'h':
                        obstacle_count++;
                        argc += 4;
                        break;
                    case 'm':
                        martian_count++;
                        argc += 5;
                        break;
                }
            }

            argc = 7;
            TelemetryMessageEventArgs.ObstacleInfo[] obstacles = new TelemetryMessageEventArgs.ObstacleInfo[obstacle_count];
            TelemetryMessageEventArgs.MartianInfo[] martians = new TelemetryMessageEventArgs.MartianInfo[martian_count];
            obstacle_count = 0;
            martian_count = 0;
            while (argc < argv.Length)
            {
                switch (argv[argc][0])
                {
                    case 'b':
                    case 'c':
                    case 'h':
                        obstacles[obstacle_count++] = new TelemetryMessageEventArgs.ObstacleInfo(
                            argv[argc][0] == 'b' ? OBSTACLE_KIND.boulder : argv[argc][0] == 'c' ? OBSTACLE_KIND.crater : OBSTACLE_KIND.home,
                            float.Parse(argv[argc + 1]),
                            float.Parse(argv[argc + 2]),
                            float.Parse(argv[argc + 3]));
                        argc += 4;
                        break;
                    case 'm':
                        martians[martian_count++] = new TelemetryMessageEventArgs.MartianInfo(
                            float.Parse(argv[argc + 1]),
                            float.Parse(argv[argc + 2]),
                            float.Parse(argv[argc + 3]),
                            float.Parse(argv[argc + 4]));
                        argc += 5;
                        break;
                }
            }

            if(TelemetryMessage != null)
                TelemetryMessage(this, new TelemetryMessageEventArgs(
                    time,
                    ms,
                    ts,
                    xpos,
                    ypos,
                    direction,
                    speed,
                    obstacles,
                    martians));
        }

        private void ParseInitialization(string[] argv)
        {
            if(InitializationMessage != null)
                InitializationMessage(this, new InitializationMessageEventArgs(
                    float.Parse(argv[1]), // dx
                    float.Parse(argv[2]), // dy
                    int.Parse(argv[3]), // time_limit
                    float.Parse(argv[4]), // min_sensor
                    float.Parse(argv[5]), // max_sensor
                    float.Parse(argv[6]), // max_speed
                    float.Parse(argv[7]), // max_turn
                    float.Parse(argv[8]))); // max_hard_turn
        } 

        void ReceiveMessage()
        {
            SocketState s = new SocketState();
            s.workSocket = m_socket;
            s.wrapper = this;
            m_socket.BeginReceive(s.buffer, 0, SocketState.BUFFER_SIZE, 0, new AsyncCallback(Read_Callback), s);
        }

        private static void Read_Callback(IAsyncResult ar)
        {
            SocketState so = (SocketState)ar.AsyncState;
            Socket s = so.workSocket;

            int read = s.EndReceive(ar);

            if (read > 0)
            {
                so.sb.Append(Encoding.ASCII.GetString(so.buffer, 0, read));
                if(so.sb.ToString().EndsWith(";"))
                {
                    lock (so.wrapper.m_lock)
                    {
                        // TODO: maybe do some parsing here?
                        string[] messages = so.sb.ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach(string message in messages)
                            so.wrapper.m_messages.Add(new Message(message));
                        so.sb.Length = 0;
                    }
                }
                s.BeginReceive(so.buffer, 0, SocketState.BUFFER_SIZE, 0,
                                         new AsyncCallback(Read_Callback), so);
            }
            else
            {
                // receive the next message
                so.wrapper.ReceiveMessage();
            }
        }

        private static void Write_Callback(IAsyncResult ar)
        {
            ServerWrapper wrapper = (ServerWrapper)ar.AsyncState;
            wrapper.m_socket.EndSend(ar);
        }
    }
}
