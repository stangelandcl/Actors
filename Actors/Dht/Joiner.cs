using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors;
using Actors.Extensions;

namespace Dht.Ring
{
    class Joiner : Actor<object>
    {
		public Joiner(DhtRing ring, ISingleRpcSender sender)
        {
            this.sender = sender;
            this.ring = ring;
        }
		ISingleRpcSender sender;
        DhtRing ring;
        public bool Joined { get; private set; }

        public void Join(IEnumerable<IActorId> peers)
        {
            if (Joined) return;
            foreach (var peer in peers)
                Post(peer);            
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
			var id = msg as IActorId;
            if (id != null)
            {
                sender.Send(id, "TryJoin");
                Post(msg);
            }
           
        }
        void Quit() 
        { 
            Joined = true; 
        }
    }
}
