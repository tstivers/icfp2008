using System;
using System.Collections.Generic;
using System.Text;
using ICFP08;

namespace ConsoleClient
{
    class Program
    {
        static ServerWrapper s = new ServerWrapper();
        static StaticObjects ws = null;
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
            foreach (TelemetryMessageEventArgs.ObstacleInfo o in tme.obstacles)
            {
                if (o.kind == OBSTACLE_KIND.home && !ws.HomeFound)
                    ws.AddHome(o.xpos, o.ypos, o.radius);
                else if (o.kind == OBSTACLE_KIND.boulder)
                    ws.AddBoulder(o.xpos, o.ypos, o.radius);
                else if (o.kind == OBSTACLE_KIND.crater)
                    ws.AddCrater(o.xpos, o.ypos, o.radius);
            }
        }

        private static void DumpTelemetryMessage(TelemetryMessageEventArgs tme)
        {
            Console.WriteLine("parsed telemetry message:");
            Console.WriteLine("  time-stamp = " + tme.time_stamp);
            Console.WriteLine("  vehicle-ctl = [" + tme.move_state.ToString() + "] [" + tme.turn_state.ToString() + "]");
            Console.WriteLine("  vehicle-x = " + tme.xpos);
            Console.WriteLine("  vehicle-y = " + tme.ypos);
            Console.WriteLine("  vehicle-dir = " + tme.direction);
            Console.WriteLine("  vehicle-speed = " + tme.speed);
            for (int i = 0; i < tme.obstacles.Length; i++)
            {
                Console.WriteLine("   object-kind[" + i + "] = " + tme.obstacles[i].kind.ToString());
                Console.WriteLine("   object-x[" + i + "] = " + tme.obstacles[i].xpos);
                Console.WriteLine("   object-y[" + i + "] = " + tme.obstacles[i].ypos);
                Console.WriteLine("   object-r[" + i + "] = " + tme.obstacles[i].radius);
            }
            for (int i = 0; i < tme.martians.Length; i++)
            {
                Console.WriteLine("   enemy-x[" + i + "] = " + tme.martians[i].xpos);
                Console.WriteLine("   enemy-y[" + i + "] = " + tme.martians[i].ypos);
                Console.WriteLine("   enemy-direction[" + i + "] = " + tme.martians[i].direction);
                Console.WriteLine("   enemy-speed[" + i + "] = " + tme.martians[i].speed);
            }
        }

        static void s_InitializationMessage(object sender, InitializationMessageEventArgs ime)
        {
            DumpInitMessage(ime);
            ws = new StaticObjects(ime.dx, ime.dy);
            ws.ObjectDiscovered += new StaticObjects.ObjectDiscoveredEventHandler(ws_ObjectDiscovered);
        }

        static void ws_ObjectDiscovered(object sender, ObjectDiscoveredEventArgs ode)
        {
            Console.WriteLine("discovered " + ode.type.ToString() + " at " + ode.position.ToString());
        }

        private static void DumpInitMessage(InitializationMessageEventArgs ime)
        {
            Console.WriteLine("parsed init message: ");
            Console.WriteLine("  dx            = " + ime.dx);
            Console.WriteLine("  dy            = " + ime.dy);
            Console.WriteLine("  time-limit    = " + ime.time_limit);
            Console.WriteLine("  min-sensor    = " + ime.min_sensor);
            Console.WriteLine("  max-sensor    = " + ime.max_sensor);
            Console.WriteLine("  max-speed     = " + ime.max_speed);
            Console.WriteLine("  max-turn      = " + ime.max_turn);
            Console.WriteLine("  max-hard-turn = " + ime.max_hard_turn);
        }

        static void s_MessageReceived(object sender, MessageEventArgs me)
        {
            if(me.message.StartsWith("I"))
                Console.WriteLine("received message: " + me.message);
        }
    }
}
