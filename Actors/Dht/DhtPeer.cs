using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors;
using KeyValueDatabase;

namespace Dht.Ring
{
    public class DhtPeer : RpcActor
    {
        public DhtPeer(IActorId id, ISingleRpcSender sender)
        {
            this.sender = sender;
            this.ring = new DhtRing(id);
            this.joiner = new Joiner(ring, sender);
            this.db = new MemoryKvpDb<string, object>();
            this.distribute = new Distributor(ring, sender, db);
			this.monitor = new PeerMonitor(ring, sender);
        }
		ISingleRpcSender sender;
        DhtRing ring;
        Joiner joiner;
		IKvpDb<string, object> db;
        Distributor distribute;
		PeerMonitor monitor;

		void SendToRing<T>(string name, T args){
			int i = 0;
            foreach (var node in ring.FindAllMax())
            {
				var msg = new RingMessage<T>{
					TimeToLive = i,
					Message = args,
					Originator = ring.SelfId,
				};
                sender.Send(node, name, msg);
                i++;
            }
		}
		void ForwardToRing<T>(string name, RingMessage<T> msg){
			int i = 0;
			foreach (var node in ring.FindAll(msg.TimeToLive, msg.Originator))
			{
				msg.TimeToLive = i;
				sender.Send(node, name, msg);
				i++;
			}
		}

        public void Join(params IActorId[] peers)
        {
            joiner.Join(peers);
        }	

		IActorId[] GetPeers(string resource)
        {
            return ring.FindClosest(resource);
        }

        void Join(IRpcMail mail, IActorId[] peers)
        {
            joiner.Join(peers);
        }


        void TryJoin(IRpcMail mail)
        {
			ring.Add(mail.From);
			IActorId[] peers = ring.Actors.Select(n=>n.Value).ToArray();
			sender.Reply(mail.From, mail.MessageId, "TryJoinReply", new object[]{peers});
			SendToRing("PeerAdded", mail.From);
        }

        void TryJoinReply(IRpcMail mail, IActorId[] peers)
        {
            joiner.JoinReply(peers);
        }

		void GetLocal(IRpcMail mail, string resource, object state)
		{
			sender.Reply(mail.From, mail.MessageId, "GetLocalReply", db.Get(resource), state);
		}

		void GetLocalReply(IRpcMail mail, object value, object state){
			var m = (IRpcMail)state;
			sender.Reply(m.From, m.MessageId, "GetReply", value);
		}

		void Get(IRpcMail mail, string resource)
        {
			var id = ring.FindClosest(resource).FirstOrDefault();
			if(id != null)
				sender.Send(id, "GetLocal", resource, mail);
			else
            	sender.Reply(mail.From, mail.MessageId, "GetReply", null);
        }

		void Added(IRpcMail mail, string resource)
        {
            var actor = ring.FindClosest(resource).First();
            if (actor.Equals(mail.From))
                db.Remove(resource); // other peer owns resource now
        }

		void Put(IRpcMail mail, string resource, object o)
        {
            db.Add(resource, o);
            distribute.Post("Put");
            sender.Reply(mail.From, mail.MessageId, "Added", resource);
        }
		void Remove(IRpcMail mail, string resource)
        {
            db.Remove(resource);
            distribute.Post("Remove");
        }

        void PeerAdded(IRpcMail mail, RingMessage<IActorId> msg)
        {
            ring.Add(msg.Message);
            distribute.Post("AddPeer");
			ForwardToRing("PeerAdded", msg);
        }

        void PeerRemoved(IRpcMail mail, RingMessage<IActorId> msg)
        {
            ring.Remove(msg.Message);
            distribute.Post("RemovePeer");
			ForwardToRing("PeerRemoved", msg);
        }       
    }
}
