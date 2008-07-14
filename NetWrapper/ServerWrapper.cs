using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace ICFP08
{
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(string message)
        {
            this.message = message;
        }

        public readonly string message;
    }
    public class InitializationMessageEventArgs : EventArgs
    {
        public InitializationMessageEventArgs(InitializationMessage m)
        {
            this.message = m;
        }

        public readonly InitializationMessage message;
    }
    public class TelemetryMessageEventArgs : EventArgs
    {
        public TelemetryMessageEventArgs(TelemetryMessage m)
        {
            this.message = m;
        }
        
        public readonly TelemetryMessage message;
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

    public class ServerWrapper
    {
        public delegate void MessageReceievedEventHandler(object sender, MessageEventArgs me);
        public delegate void InitializationMessageEventHandler(object sender, InitializationMessageEventArgs ime);
        public delegate void TelemetryMessageEventHandler(object sender, TelemetryMessageEventArgs tme);
        public delegate void EventMessageEventHandler(object sender, EventMessageEventArgs ae);
        public delegate void EndOfRunMessageEventHandler(object sender, EndOfRunMessageEventArgs ee);

        public event MessageReceievedEventHandler MessageReceived;
        public event InitializationMessageEventHandler InitializationMessage;
        public event TelemetryMessageEventHandler TelemetryMessage;
        public event EventMessageEventHandler CrashMessage;
        public event EventMessageEventHandler CraterMessage;
        public event EventMessageEventHandler KilledMessage;
        public event EventMessageEventHandler SuccessMessage;
        public event EndOfRunMessageEventHandler EndOfRunMessage;

        protected Socket m_socket;

        protected Object m_messagelock = new Object();
        public List<string> m_messages = new List<string>();

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
                tempSocket.NoDelay = true; // DISABLE NAGLE
                tempSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1); // NO REALLY

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
            m_socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1); // NO REALLY
            
            // start receiving data
            ReceiveMessage();
        }

        public void Disconnect()
        {
            if(m_socket.Connected)
                m_socket.Disconnect(false);
        }

        public void ProcessMessages()
        {
            lock(m_messagelock)
            {
                foreach(string message in m_messages)
                {
                    if (MessageReceived != null)
                        MessageReceived(this, new MessageEventArgs(message));
                    ParseMessage(message);
                }
                m_messages.Clear();
            }
        }

        public void SendCommand(MoveType move, TurnType turn)
        {
            if (!Connected)
                return;

            string command = "";
            if(move == MoveType.Accelerate)
                command += "a";
            else if(move == MoveType.Brake)
                command += "b";
            if(turn == TurnType.Left || turn == TurnType.HardLeft)
                command += "l";
            else if(turn == TurnType.Right || turn == TurnType.HardRight)
                command += "r";
            command += ";";
            SendMessage(command);
        }

        public void SendMessage(string m)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(m);
            m_socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Write_Callback), this);
        }

        private void ParseMessage(string p)
        {
            if (p.Trim().Length == 0)
                return;
            string[] tokens = p.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
                return;

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
            MoveType ms = MoveType.Roll;
            switch(argv[2][0])
            {
                case 'a':
                    ms = MoveType.Accelerate;
                    break;
                case 'b':
                    ms = MoveType.Brake;
                    break;
                case '-':
                    ms = MoveType.Roll;
                    break;
            }

            TurnType ts = TurnType.Straight;
            switch(argv[2][1])
            {
                case 'L':
                    ts = TurnType.HardLeft;
                    break;
                case 'l':
                    ts = TurnType.Left;
                    break;
                case '-':
                    ts = TurnType.Straight;
                    break;
                case 'r':
                    ts = TurnType.Right;
                    break;
                case 'R':
                    ts = TurnType.HardRight;
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
            ObstacleMessage[] obstacles = new ObstacleMessage[obstacle_count];
            MartianMessage[] martians = new MartianMessage[martian_count];
            obstacle_count = 0;
            martian_count = 0;
            while (argc < argv.Length)
            {
                switch (argv[argc][0])
                {
                    case 'b':
                    case 'c':
                    case 'h':
                        obstacles[obstacle_count++] = new ObstacleMessage(
                            argv[argc][0] == 'b' ? ObstacleType.Boulder : argv[argc][0] == 'c' ? ObstacleType.Crater : ObstacleType.Home,
                            float.Parse(argv[argc + 1]),
                            float.Parse(argv[argc + 2]),
                            float.Parse(argv[argc + 3]));
                        argc += 4;
                        break;
                    case 'm':
                        martians[martian_count++] = new MartianMessage(
                            float.Parse(argv[argc + 1]),
                            float.Parse(argv[argc + 2]),
                            float.Parse(argv[argc + 3]),
                            float.Parse(argv[argc + 4]));
                        argc += 5;
                        break;
                }
            }

            if(TelemetryMessage != null)
                TelemetryMessage(this, new TelemetryMessageEventArgs(new TelemetryMessage(
                    time,
                    ms,
                    ts,
                    xpos,
                    ypos,
                    direction,
                    speed,
                    obstacles,
                    martians)));
        }

        private void ParseInitialization(string[] argv)
        {
            if(InitializationMessage != null)
                InitializationMessage(this, new InitializationMessageEventArgs(new InitializationMessage(
                    float.Parse(argv[1]), // dx
                    float.Parse(argv[2]), // dy
                    int.Parse(argv[3]), // time_limit
                    float.Parse(argv[4]), // min_sensor
                    float.Parse(argv[5]), // max_sensor
                    float.Parse(argv[6]), // max_speed
                    float.Parse(argv[7]), // max_turn
                    float.Parse(argv[8])))); // max_hard_turn
        } 

        void ReceiveMessage()
        {
            if (!m_socket.Connected)
                return;

            SocketState s = new SocketState();
            s.workSocket = m_socket;
            s.wrapper = this;
            try
            {
                m_socket.BeginReceive(s.buffer, 0, SocketState.BUFFER_SIZE, 0, new AsyncCallback(Read_Callback), s);
            }
            catch (Exception)
            {
            }
        }

        private static void Read_Callback(IAsyncResult ar)
        {
            SocketState so = (SocketState)ar.AsyncState;
            Socket s = so.workSocket;
            int read = 0;
            try
            {
                read = s.EndReceive(ar);
            }
            catch (Exception)
            { 
            }

            if (read > 0)
            {
                so.sb.Append(Encoding.ASCII.GetString(so.buffer, 0, read));
                if(so.sb.ToString().EndsWith(";"))
                {
                    lock (so.wrapper.m_messagelock)
                    {
                        // TODO: maybe do some parsing here?
                        string[] messages = so.sb.ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        so.wrapper.m_messages.AddRange(messages);
                        so.sb.Length = 0;
                    }
                }
                try
                {
                    s.BeginReceive(so.buffer, 0, SocketState.BUFFER_SIZE, 0,
                         new AsyncCallback(Read_Callback), so);
                }
                catch (System.Exception)
                {
                	// TODO: handle disconnect
                }
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
