using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Dht;

namespace Actors.Builtins.Actors.Dht
{
    class RingSender
    {
        public RingSender(DistributedActor actor, DhtRing ring)
        {
            this.actor = actor;
            this.ring = ring;            
        }
        DhtRing ring;
        DistributedActor actor;

        public void Forward(Mail m)
        {
            var meta = (DhtMetadata)m.Args[0];
            var ttl = meta.TimeToLive;
            var sendTo = ring.FindAll(meta.TimeToLive - 1, meta.Originator);
            int i = 0;
            foreach (var to in sendTo)
            {
                var args = new object[m.Args.Length];
                args[0] = new DhtMetadata { Originator = meta.Originator, TimeToLive = i++ };
                Array.Copy(m.Args, 1, args, 1, args.Length - 1);
                actor.Node.Send(to, actor.Box.Id, m.Name, args);
            }
        }

        public void SendToRing(FunctionId name, params object[] args)
        {
            var sendTo = ring.FindAllMax();
            int i = 0;
            foreach (var to in sendTo)
            {
                var toArgs = new object[args.Length + 1];
                toArgs[0] = new DhtMetadata { Originator = actor.Box.Id, TimeToLive = i++ };
                for (int j = 1; j < toArgs.Length; j++)
                    toArgs[j] = args[j - 1];
                actor.Node.Send(to, actor.Box.Id, name, toArgs);
            }
        }
    }
}
