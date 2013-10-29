using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueDatabase;
using Actors;

namespace Dht.Ring
{
    class Distributor : Actor
    {
        public Distributor(DhtRing ring, ISender sender, IKvpDb<IResource,object> db)
        {
            this.ring = ring;
            this.db = db;
            this.sender = sender;
        }
        ISender sender;
        DhtRing ring;
        IKvpDb<IResource, object> db;       

        protected override void HandleMessage(object message)
        {
            foreach (var key in db.Keys)
            {
                var peer = ring.FindClosest(key, minCount: 1, maxCount: 1).First();
                if (!peer.Equals(ring.SelfId))
                    sender.Send(peer, "Put", key, db.Get(key));
            }
        }
    }
}
