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
            DistributedActor actor,
            DhtRing ring,           
            Action<Action, int> run,
            RingSender sender)
        {
            this.ring = ring;
            this.actor = actor;                       
            this.Run = run;
            this.sender = sender;
            Loop();
        }
        static readonly TimeSpan nodeCheckTime = TimeSpan.FromSeconds(5);
        static readonly int nodeCheckIntervalMs = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
        RingSender sender;
        DhtRing ring;
        DistributedActor actor;      
        Action<Action, int> Run;
        void Loop()
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
            var msg = actor.Node.Send(node, actor.Box.Id, "IsAlive");
            var mail = actor.Box.WaitFor(msg, nodeCheckTime);
            if (mail == null)
                sender.SendToRing("PeerRemoved", node);
        }
    }
}
