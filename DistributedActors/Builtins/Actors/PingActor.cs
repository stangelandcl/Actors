using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Examples
{
    public class PingActor : DistributedActor
    {
        public PingActor(string shortName = "System.Ping")
            : base(shortName)
        { }
        
        void Ping(Mail m, byte[] data)
        {
            Node.Reply(m, data);
        }
    }
}
