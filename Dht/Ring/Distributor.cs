using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueDatabase;

namespace Dht.Ring
{
    class Distributor
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

        public void Run()
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
