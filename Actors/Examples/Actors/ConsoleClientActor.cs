using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RemoteConsole;
using System.Threading.Tasks;
using Actors.Tasks;

namespace Actors.Examples.Actors
{
    public class ConsoleClientActor : Actor
    {
        public ConsoleClientActor(string shortname)
            : base(shortname) 
        {
            TaskEx.Delay(100).ContinueWith(GetKeys);
        }

        ActorId remote;
        IConsole console = new Win32Console();
        void ScreenUpdate(Mail m, Screen screen, CursorPosition position)
        {
            console.Screen = screen;
            console.CursorPosition = position;
            remote = m.From;    
        }

        void GetKeys(Task task)
        {
            if (!remote.IsEmpty)
            {
                var keys = console.Keys;
                if(keys.Any())
                    Node.Send(remote, Box.Id, MessageId.New(), "Keys",keys);
            }
            TaskEx.Delay(100).ContinueWith(GetKeys);
        }
    }
}
