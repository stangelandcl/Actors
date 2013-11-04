using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cls.Actors
{
    public class PingActor : DistributedActor
    {
        public PingActor(string shortName = "System.Ping")
            : base(shortName)
        { }
        
        void Ping(IMail m, byte[] data)
        {
            Node.Reply(m, data);
        }
    }
}
