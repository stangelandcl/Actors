using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cls.Extensions;

namespace Cls.Actors
{
    public class DhtPeer : RpcActor
    {
        public DhtPeer(Log log, IActorId id, ISingleRpcSender sender)
        {
			this.log = log;
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
		Log log;

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
			log.Info("Join", peers.ToDelimitedString());
            joiner.Join(peers);
        }	

		IActorId[] GetPeers(string resource)
        {
            return ring.FindClosest(resource);
        }

        void Join(IRpcMail mail, IActorId[] peers)
        {
			log.Info("Join", peers.ToDelimitedString(), mail.From);
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
			log.Info("TryJoinReply", peers.ToDelimitedString());
            joiner.JoinReply(peers);
        }

		void GetLocal(IRpcMail mail, string resource, object state)
		{
			var obj = db.Get(resource);
			log.Info("GetLocal", resource, obj);
			sender.Reply(mail.From, mail.MessageId, "GetLocalReply", obj , state);
		}

		void GetLocalReply(IRpcMail mail, object value, object state){
			var m = (IRpcMail)state;
			sender.Reply(m.From, m.MessageId, "GetReply", value);
		}

		void Get(IRpcMail mail, string resource)
        {
			log.Info("Get", resource, mail.From);
			var id = ring.FindClosest(resource).FirstOrDefault();
			if(id != null)
				sender.Send(id, "GetLocal", resource, mail);
			else
            	sender.Reply(mail.From, mail.MessageId, "GetReply", null);
        }

		void Added(IRpcMail mail, string resource)
        {
			log.Info("Added", resource, mail.From);
            var actor = ring.FindClosest(resource).First();
            if (actor.Equals(mail.From))
                db.Remove(resource); // other peer owns resource now
        }

		void Put(IRpcMail mail, string resource, object o)
        {
			log.Info("Put", resource, o, mail.From);
            db.Add(resource, o);
            distribute.Post("Put");
            sender.Reply(mail.From, mail.MessageId, "Added", resource);
        }
		void Remove(IRpcMail mail, string resource)
        {
			log.Info("Remove", resource, mail.From);
            db.Remove(resource);
            distribute.Post("Remove");
        }

        void PeerAdded(IRpcMail mail, RingMessage<IActorId> msg)
        {
			log.Info("PeerAdded", msg.Message, mail.From);
            ring.Add(msg.Message);
            distribute.Post("AddPeer");
			ForwardToRing("PeerAdded", msg);
        }

        void PeerRemoved(IRpcMail mail, RingMessage<IActorId> msg)
        {
			log.Info("PeerRemoved", msg.Message, mail.From);
            ring.Remove(msg.Message);
            distribute.Post("RemovePeer");
			ForwardToRing("PeerRemoved", msg);
        }       
    }
}
