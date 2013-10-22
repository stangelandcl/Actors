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
            TaskEx.Delay(50).ContinueWith(Snapshot);
        }

        Win32HiddenConsole process;
        List<ActorId> actors = new List<ActorId>();
        Screen lastScreen;

        void Snapshot(Task task)
        {
            if (!process.IsAlive)
                Die("Process terminated");
            ActorId[] a;
            lock (actors)
                a = actors.ToArray();
            var screen = process.Console.Screen;
            if (lastScreen != null && lastScreen.Equals(screen))
                return;
            lastScreen = screen;
            foreach (var actor in a)
                Node.Send(actor, Box.Id, MessageId.New(), "ScreenUpdate", screen, process.Console.CursorPosition);
            TaskEx.Delay(50).ContinueWith(Snapshot);
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

    }
}
