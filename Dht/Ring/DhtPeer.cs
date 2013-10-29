using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors;
using System;
using KeyValueDatabase;

namespace Dht.Ring
{
    class DhtPeer : RpcActor
    {
        public DhtPeer(IActorId id, ISender sender)
        {
            this.sender = sender;
            this.ring = new DhtRing(id);
            this.joiner = new Joiner(ring, sender);
            this.db = new MemoryKvpDb<IResource, object>();
            this.distribute = new Distributor(ring, sender, db);
        }
        ISender sender;
        DhtRing ring;
        Joiner joiner;
        IKvpDb<IResource, object> db;
        Distributor distribute;

        public void JoinDht(IEnumerable<IActorId> peers)
        {
            joiner.Join(peers);
        }

        IActorId[] GetPeers(IResource resource)
        {
            return ring.FindClosest(resource);
        }

        void JoinDht(IRpcMail mail, IActorId[] peers)
        {
            joiner.Join(peers);
        }


        void Join(IRpcMail mail)
        {
            sender.Send(mail.From, "JoinReply", ring.Actors.Select(n=>n.Value).ToArray());
        }

        void JoinReply(IRpcMail mail, IActorId[] peers)
        {
            joiner.JoinReply(peers);
        }      

        void Get(IRpcMail mail, IResource resource)
        {
            sender.Send(mail.From, "GetReply", resource, db.Get(resource));
        }

        void Added(IRpcMail mail, IResource resource)
        {
            var actor = ring.FindClosest(resource).First();
            if (actor.Equals(mail.From))
                db.Remove(resource); // other peer owns resource now
        }

        void Put(IRpcMail mail, IResource resource, object o)
        {
            db.Add(resource, o);
            distribute.Send("Put");
            sender.Send(mail.From, "Added", resource);
        }
        void Remove(IRpcMail mail, IResource resource)
        {
            db.Remove(resource);
            distribute.Send("Remove");
        }

        void Add(IRpcMail mail, IActorId peer)
        {
            ring.Add(peer);
            distribute.Send("AddPeer");
        }

        void Remove(IRpcMail mail, IActorId peer)
        {
            ring.Remove(peer);
            distribute.Send("RemovePeer");
        }       
    }
}
