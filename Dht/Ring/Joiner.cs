﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors;

namespace Dht.Ring
{
    class Joiner : Actor<object>
    {
        public Joiner(DhtRing ring, ISender sender)
        {
            this.sender = sender;
            this.ring = ring;
        }
        ISender sender;
        DhtRing ring;
        public bool Joined { get; private set; }

        public void Join(IEnumerable<IActorId> peers)
        {
            if (Joined) return;
            foreach (var peer in peers)
                Send(peer);            
        }

        public void JoinReply(IActorId[] peers)
        {
            ring.AddRange(peers);
            Quit();
        }

        protected override void Next()
        {
            if (Joined) return;
            TaskEx.Delay(5000).ContinueWith(Execute);
        }

        protected override void HandleMessage(object msg)
        {
            if (msg.GetType() == typeof(IActorId))
            {
                sender.Send((IActorId)msg, "Join");
                Send(msg);
            }
           
        }
        void Quit() { Joined = true; }
    }
}
