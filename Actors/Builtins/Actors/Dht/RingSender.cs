using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Dht;

namespace Actors.Builtins.Actors.Dht
{
    class RingSender
    {
        public RingSender(Node node, MailBox box, DhtRing ring)
        {
            this.Node = node;
            this.ring = ring;
            this.Box = box;
        }
        DhtRing ring;
        Node Node;
        MailBox Box;
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
                Node.Send(to, Box.Id, m.Name, args);
            }
        }

        public void SendToRing(FunctionId name, params object[] args)
        {
            var sendTo = ring.FindAllMax();
            int i = 0;
            foreach (var to in sendTo)
            {
                var toArgs = new object[args.Length + 1];
                toArgs[0] = new DhtMetadata { Originator = Box.Id, TimeToLive = i++ };
                for (int j = 1; j < toArgs.Length; j++)
                    toArgs[j] = args[j - 1];
                Node.Send(to, Box.Id, name, toArgs);
            }
        }
    }
}
