using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Examples.Actors
{
    public class Shell : Actor
    {
        public Shell()
            : base("System.Shell")
        { }

        protected virtual void RunConsole(Mail mail, string exe, string[] args, ActorId shell)
        {
            var console = new ConsoleProcessActor(exe, exe, args);
            Node.Add(console);
            console.AttachConsole(shell);
            Node.Reply(mail, console.Box.Id);
        }
                
    }
}
