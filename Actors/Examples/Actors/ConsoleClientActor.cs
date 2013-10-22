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
            Run(GetKeys, 100);
        }

        ActorId remote;
        Win32Console console = new Win32Console();
        void ScreenUpdate(Mail m, Screen screen, CursorPosition position)
        {
            console.Screen = screen;
            console.CursorPosition = position;
            remote = m.From;    
        }

        void GetKeys()
        {
            if (!remote.IsEmpty)
            {
                var keys = console.Keys;
                if(keys.Any())
                    Node.Send(remote, Box.Id, "Keys",keys);
            }
            Run(GetKeys, 100);            
        }
        protected override void Disposing(bool b)
        {
            console.Dispose();
            base.Disposing(b);
        }
    }
}
