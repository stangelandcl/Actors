using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors;
using Actors.Extensions;

namespace Dht.Ring
{
    class PeerMonitor
    {
		public PeerMonitor(DhtRing ring, ISingleRpcSender sender)
        {
            this.ring = ring;
            this.sender = sender;
            TaskEx.Run(Check);
        }
        DhtRing ring;
		ISingleRpcSender sender;

        void Check()
        {
//            var pred = ring.Predecessor;
//            if (pred != null)
//            {
//                //TODO: add SendReceive()/wait for reply.
//                sender.Send(pred, "Ping");
//                var result = false;
//                if (!result)
//                {
//                    ring.Remove(pred);
//                    int i = 0;
//                    foreach (var node in ring.FindAllMax())
//                    {
//                        sender.Send(node, "PeerRemoved", pred, i);
//                        i++;
//                    }
//                }
//            }
            Run();
        }

        private void Run()
        {
            TaskEx.Delay(5 * 1000 * 60).ContinueWith(Check);
        }
    }
}
