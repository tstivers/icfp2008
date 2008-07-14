using System;
using System.Collections.Generic;
using System.Text;
using ICFP08;
using System.Threading;

namespace ConsoleClient
{
    class Program
    {
        static ServerWrapper s = new ServerWrapper();
        static WorldState ws;
        static RoverController rc;
        static int runs = 0;

        static void Main(string[] args)
        {
            //s.MessageReceived += new ServerWrapper.MessageReceievedEventHandler(s_MessageReceived);
            s.InitializationMessage += new ServerWrapper.InitializationMessageEventHandler(s_InitializationMessage);
            s.TelemetryMessage += new ServerWrapper.TelemetryMessageEventHandler(s_TelemetryMessage);
            s.EndOfRunMessage += new ServerWrapper.EndOfRunMessageEventHandler(s_EndOfRunMessage);
            s.Connect(args[0], int.Parse(args[1]));
            while (s.Connected)
            {
                if(s.ProcessMessages() == 0)
                    Thread.Sleep(1);
            }
        }

        static void s_EndOfRunMessage(object sender, EndOfRunMessageEventArgs ee)
        {
            Console.WriteLine("Got end of run message! [time = " + ee.time_stamp + ", score = " + ee.score + "]");
            System.GC.Collect();
            //ws.ForgetStuff();
            runs++;
            if (runs == 5)
                s.Disconnect();
        }

        static void s_TelemetryMessage(object sender, TelemetryMessageEventArgs tme)
        {
            ws.UpdateWorldState(tme.message);
            rc.DoUpdate();
        }

        static void s_InitializationMessage(object sender, InitializationMessageEventArgs ime)
        {
            ws = new WorldState(ime.message);
            rc = new StupidController(ws, s);
        }

        static void s_MessageReceived(object sender, MessageEventArgs me)
        {
            if(me.message.StartsWith("I"))
                Console.WriteLine("received message: " + me.message);
        }
    }
}
