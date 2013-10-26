using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Dht;
using Dht;

namespace Actors.Builtins.Actors.Dht
{
    public class DhtActor : Actor
    {
        public DhtActor(ILocalData cache, string shortname = "System.Dht")
            : base(shortname)
        {
            this.cache = cache;
            
        }
 
        ILocalData cache;
        DhtRing ring;
        RingSender sender;
        HeartbeatMonitor monitor;
        public event Action<string, string> SubscriptionHit;

        public override void AttachNode(Node node)
        {
            base.AttachNode(node);
            this.ring = new DhtRing(Box.Id);
            sender = new RingSender(Node, Box, ring);
            monitor = new HeartbeatMonitor(Node, Box, ring, Run, sender);
            monitor.Loop();
        }
      
        void JoinReply(Mail m, DhtMetadata meta, ActorId[] actors, KeyValuePair<string, byte[]>[] data)
        {
            ring.AddRange(actors.Select(n=>(DhtId)n));
            cache.AddRange(data);
        }

        void Join(Mail m, DhtMetadata meta)
        {
            Node.Send(meta.Originator, Box.Id, "JoinReply", ring.Actors, cache.Data);
            sender.SendToRing("PeerAdded", m.From);          
        }           

        void PeerAdded(Mail mail, DhtMetadata meta, ActorId peer)
        {
            ring.Add(peer);
            sender.Forward(mail);
        }

        void IsNodeAlive(Mail mail)
        {
            Node.Reply(mail);
        }

        void PeerRemoved(Mail mail, DhtMetadata meta, ActorId peer)
        {
            foreach (var kvp in cache.Data)
            {
                var peers = ring.FindClosest(kvp.Key);
                var index = peers.IndexOf(peer);
                if (index >= 0)
                {
                    if (--index < 0)
                        index = ring.Actors.Length - 1;
                    Node.Send(ring.Actors[index], Box.Id, "AddLocal", kvp.Key, kvp.Value);
                }
            }
            ring.Remove(peer);
            sender.Forward(mail);
        }

        void Subscribe(Mail mail,DhtOperation operations, string keyRegex)
        {
			var subscribe = new Subscription(mail.From, operations, keyRegex);
            ring.Subscribe(subscribe);
            sender.SendToRing("SubscribeContinue", subscribe);
        }

		void Unsubscribe(Mail mail, DhtOperation operations, string keyRegex)
        {
			var subscribe = new Subscription(mail.From, operations, keyRegex);
            ring.Unsubscribe(subscribe);
            sender.SendToRing("UnubscribeContinue", subscribe);
        }

        void SubscribeContinue(Mail mail, DhtMetadata meta, Subscription subscription)
        {
            ring.Subscribe(subscription);
            sender.Forward(mail);
        }

        void UnsubscribeContinue(Mail mail, DhtMetadata meta, Subscription subscription)
        {
            ring.Unsubscribe(subscription);
            sender.Forward(mail);
        }

        void SubscriptionMatched(Mail mail, string operation, string value)
        {
            SubscriptionHit.FireEventAsync(operation, value);
        }

        void Add(Mail mail, string key, byte[] value)
        {
            foreach (var to in ring.FindClosest(key))
            {
                if (to.Equals((DhtId)Box.Id)) cache.Add(key, value);
                else Node.Send(to, Box.Id, "AddLocal", key, value);
            }
            foreach (var subscription in ring.GetMatches(DhtOperation.Add, key))
                Node.Send(subscription.Node, Box.Id, "SubscriptionMatched", "Add", key);
            Node.Reply(mail, "true");
        }

        void Get(Mail mail, string key)
        {
            var msgIds = new List<MessageId>();
            foreach (var to in ring.FindClosest(key))
            {
                if (to.Equals((DhtId)Box.Id)) Node.Reply(mail, cache.Get(key));
                else msgIds.Add(Node.Send(to, Box.Id, "GetLocal", key));
            }
            foreach (var subscription in ring.GetMatches(DhtOperation.Get, key))
                Node.Send(subscription.Node, Box.Id, "SubscriptionMatched", "Get", key);
            var ids = msgIds.ToHashSet();
            var reply = Box.CheckFor(n=> ids.Contains(n.MessageId), TimeSpan.FromSeconds(5));
            var result = reply.Coalesce(n => n.Args[0]) as byte[];          
            Node.Reply(mail, result);
        }


        void Remove(Mail mail, string key)
        {
            foreach (var to in ring.FindClosest(key))
            {
                if (to.Equals((DhtId)Box.Id)) cache.Remove(key);
                else Node.Send(to, Box.Id, "RemoveLocal", key);
            }
            foreach (var subscription in ring.GetMatches(DhtOperation.Remove, key))
                Node.Send(subscription.Node, Box.Id, "SubscriptionMatched", "Remove", key);
            Node.Reply(mail, "true");
        }

        void AddLocal(Mail m, string key, byte[] value)
        {
            cache.Add(key, value);
        }

        void GetLocal(Mail m, string key)
        {            
            Node.Reply(m, cache.Get(key));
        }      

        void RemoveLocal(Mail m, string key)
        {
            cache.Remove(key);           
        }
    }
}
