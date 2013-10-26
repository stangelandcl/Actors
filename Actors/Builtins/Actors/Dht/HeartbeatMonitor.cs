using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Dht;

namespace Actors.Builtins.Actors.Dht
{
    class HeartbeatMonitor
    {
        public HeartbeatMonitor(
            Node node,
            MailBox box,
            DhtRing ring,           
            Action<Action, int> run,
            RingSender sender)
        {
            this.ring = ring;
            this.Box = box;            
            this.ring = ring;
            this.Run = run;
            this.sender = sender;
        }
        static readonly TimeSpan nodeCheckTime = TimeSpan.FromSeconds(5);
        static readonly int nodeCheckIntervalMs = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
        RingSender sender;
        DhtRing ring;
        MailBox Box;
        Node Node;        
        Action<Action, int> Run;
        public void Loop()
        {
            Run(CheckAlive, nodeCheckIntervalMs);
        }

        void CheckAlive()
        {
            try
            {
                var successor = ring.Successor;
                if (successor.HasValue)
                    CheckAlive(successor.Value);
            }
            finally
            {
                Loop();
            }
        }

        private void CheckAlive(ActorId node)
        {
            var msg = Node.Send(node, Box.Id, "IsAlive");
            var mail = Box.WaitFor(msg, nodeCheckTime);
            if (mail == null)
                sender.SendToRing("PeerRemoved", node);
        }
    }
}
