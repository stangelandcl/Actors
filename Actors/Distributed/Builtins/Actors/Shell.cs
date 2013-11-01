using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors
{
    public class Shell : DistributedActor
    {
        public Shell()
            : base("System.Shell")
        { }

        protected virtual void RunConsole(IMail mail, string exe, string[] args, ActorId shell)
        {
            var console = new ConsoleProcessActor(exe, exe, args);
            Node.Add(console);
            console.AttachConsole(shell);
            Node.Reply(mail, console.Id);
        }
                
    }
}
