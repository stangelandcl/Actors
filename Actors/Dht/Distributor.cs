using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyValueDatabase;
using Actors;
using Actors.Extensions;

namespace Dht.Ring
{
    class Distributor : Actor
    {
		public Distributor(DhtRing ring, ISingleRpcSender sender, IKvpDb<string,object> db)
        {
            this.ring = ring;
            this.db = db;
            this.sender = sender;
        }
		ISingleRpcSender sender;
        DhtRing ring;
		IKvpDb<string, object> db;  			 

        protected override void HandleMessage(object message)
        {
			bool wrong = false;
            foreach (var key in db.Keys)
            {
                var peer = ring.FindClosest(key, minCount: 1, maxCount: 1).First();
                if (!peer.Equals(ring.SelfId)){
					var value = db.Get(key);
                    sender.Send(peer, "Put", key, value);
					wrong = true;
				}
            }
			if(wrong) 
				TaskEx.Delay(5000).ContinueWith( ()=> Post("Wrong"));
        }
    }
}
