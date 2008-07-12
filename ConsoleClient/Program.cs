using System;
using System.Collections.Generic;
using System.Text;
using ICFP08;

namespace ConsoleClient
{
    class Program
    {
        static ServerWrapper s = new ServerWrapper();

        static void Main(string[] args)
        {
            //s.MessageReceived += new ServerWrapper.MessageReceievedEventHandler(s_MessageReceived);
            s.InitializationMessage += new ServerWrapper.InitializationMessageEventHandler(s_InitializationMessage);
            s.TelemetryMessage += new ServerWrapper.TelemetryMessageEventHandler(s_TelemetryMessage);
            s.EndOfRunMessage += new ServerWrapper.EndOfRunMessageEventHandler(s_EndOfRunMessage);
            s.KilledMessage += new ServerWrapper.EventMessageEventHandler(s_KilledMessage);
            s.CrashMessage += new ServerWrapper.EventMessageEventHandler(s_CrashMessage);
            s.CraterMessage += new ServerWrapper.EventMessageEventHandler(s_CraterMessage);
            s.Connect("172.16.1.44", 17676);
            while (s.Connected)
            {
                s.ProcessMessages();
            }
        }

        static void s_CraterMessage(object sender, EventMessageEventArgs ae)
        {
            Console.WriteLine("Fell into a crater!");
        }

        static void s_CrashMessage(object sender, EventMessageEventArgs ae)
        {
            Console.WriteLine("Crashed!");
        }

        static void s_KilledMessage(object sender, EventMessageEventArgs ae)
        {
            Console.WriteLine("Killed by a martian!");
        }

        static void s_EndOfRunMessage(object sender, EndOfRunMessageEventArgs ee)
        {
            Console.WriteLine("Got end of run message! [time = " + ee.time_stamp + ", score = " + ee.score + "]");
        }

        static void s_TelemetryMessage(object sender, TelemetryMessageEventArgs tme)
        {
        }

        static void s_InitializationMessage(object sender, InitializationMessageEventArgs ime)
        {
        }

        static void s_MessageReceived(object sender, MessageEventArgs me)
        {
            if(me.message.StartsWith("I"))
                Console.WriteLine("received message: " + me.message);
        }
    }
}
