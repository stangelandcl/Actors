using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors;
using System;
using KeyValueDatabase;

namespace Dht.Ring
{
    class DhtPeer : MailActor
    {
        public DhtPeer(IActorId id, ISender sender)
        {     
            this.ring = new DhtRing(id);
            this.joiner = new Joiner(ring, sender);
        }       
        DhtRing ring;
        Joiner joiner;
        IKvpDb<IResource, object> db;

        public void Join(IEnumerable<IActorId> peers)
        {
            joiner.Join(peers);
        }

        public IActorId[] GetPeers(IResource resource)
        {
            return ring.FindClosest(resource);
        }

        public object Get(IResource resource){
        }

        public void Put(IResource resource, object o)
        {
        }
        public void Remove(IResource resource)
        {
        }

        public void Add(IActorId peer)
        {
            ring.Add(peer);
        }

        public void Remove(IActorId peer)
        {
            ring.Remove(peer);
        }

        protected override void HandleMessage(IMail message)
        {
            if (message.Message[0] == "Joined")
            {
                joiner.JoinReply((IActorId[])message.Message[1]);
            }
        }
    }
}
