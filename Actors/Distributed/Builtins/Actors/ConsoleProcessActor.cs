using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace Cls.Actors
{
    public class ConsoleProcessActor : DistributedActor
    {
        public ConsoleProcessActor(string shortname, string exe, params string[] args)
            : base(shortname)
        {
            process = new Win32HiddenConsole(exe, args);
            Loop();
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
                    Node.Send(actor, Id, "ScreenUpdate", screen, process.Console.CursorPosition);
            }
            Loop();
        }

        private void Loop()
        {
            Run(Snapshot, 50);
        }

        void Keys(IMail m, KeyPress[] keys)
        {
            process.Console.Keys = keys;
        }

        void Attach(IMail m, ActorId sendTo)
        {
            AttachConsole(sendTo);
        }

        internal void AttachConsole(ActorId sendTo)
        {
            lock (actors)
                actors.Add(sendTo);            
        }

        void Detach(IMail m, ActorId sendTo)
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
