using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RemoteConsole;
using Actors.Tasks;
using System.Threading.Tasks;

namespace Actors.Examples.Actors
{
    public class ConsoleProcessActor : Actor
    {
        public ConsoleProcessActor(string shortname, string exe, params string[] args)
            : base(shortname)
        {
            process = new Win32HiddenConsole(exe, args);
            Run(Snapshot, 100);
        }

        Win32HiddenConsole process;
        List<ActorId> actors = new List<ActorId>();
        Screen lastScreen;
        DateTime lastUpdate;

        void Snapshot()
        {
            if (!process.IsAlive)
            {
                Die("Process terminated");
                return;
            }
             
            ActorId[] a;
            lock (actors)
                a = actors.ToArray();
            var screen = process.Console.Screen;
            var now = DateTime.Now;
            if (lastScreen == null || !lastScreen.Equals(screen) || 
                now - lastUpdate > TimeSpan.FromSeconds(5))
            {
                lastUpdate = now;
                lastScreen = screen;
                foreach (var actor in a)
                    Node.Send(actor, Box.Id, "ScreenUpdate", screen, process.Console.CursorPosition);
            }
            Run(Snapshot, 100);
        }

        void Keys(Mail m, KeyPress[] keys)
        {
            process.Console.Keys = keys;
        }

        void Attach(Mail m, ActorId sendTo)
        {
            lock (actors)
                actors.Add(sendTo);
        }

        void Detach(Mail m, ActorId sendTo)
        {
            lock (actors)
                actors.Remove(sendTo);
        }

        protected override void Disposing(bool b)
        {
            if (this.process != null)
                process.Dispose();
            process = null;
            base.Disposing(b);
        }

    }
}
