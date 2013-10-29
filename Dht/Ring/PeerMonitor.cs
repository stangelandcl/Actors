using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dht.Ring
{
    class PeerMonitor
    {
        public PeerMonitor(DhtRing ring, ISender sender)
        {
            this.ring = ring;
            this.sender = sender;
            TaskEx.Run(Check);
        }
        DhtRing ring;
        ISender sender;

        void Check()
        {
            var pred = ring.Predecessor;
            if (pred != null)
            {
                var result = sender.SendReceive<bool>(pred.Value, "Ping");
                if (!result)
                {
                    ring.Remove(pred.Value);
                    int i = 0;
                    foreach (var node in ring.FindAllMax())
                    {
                        sender.Send(node, "PeerRemoved", pred.Value, i);
                        i++;
                    }
                }
            }
            Run();
        }

        private void Run()
        {
            TaskEx.Delay(5 * 1000 * 60).ContinueWith(Check);
        }
    }
}
