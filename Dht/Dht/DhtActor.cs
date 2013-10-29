using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Dht;
using Dht;
using KeyValueDatabase;
using System.Threading;

namespace Actors.Builtins.Actors.Dht
{
    public class DhtActor : DistributedActor
    {
        public DhtActor(IDhtBackend cache, string shortname = "System.Dht")
            : base(shortname)
        {
            this.cache = cache;
        }

        IDhtBackend cache;
        DhtRing ring;
        RingSender sender;
        HeartbeatMonitor monitor;
        DhtJoiner joiner;
        public event Action<string, string> SubscriptionHit;

        public override void AttachNode(Node node)
        {
            base.AttachNode(node);

            this.ring = new DhtRing(Box.Id, cache);
            sender = new RingSender(this, ring);
            monitor = new HeartbeatMonitor(this, ring, Run, sender);           
            joiner = new DhtJoiner(this, Run);
            joiner.AddRange(cache.Peers);            
        }

        void DhtJoinReply(Mail m, ActorId[] actors, KeyValuePair<object,object>[] data)
        {
            joiner.Joined();
            ring.AddRange(actors);
            cache.AddRange(data);
        }

        public void Join(params ActorId[] bootstraps)
        {
            joiner.AddRange(bootstraps);
        }

        void Join(Mail mail, ActorId[] bootstraps)
        {
            Join(bootstraps);
            //while (!joiner.IsJoined)
            //    Thread.Sleep(100);
            //Node.Reply(mail, true);
        }

        void DhtJoin(Mail m)
        {
            // we aren't joined to the DHT yet.
            // don't allow others to join to us.
            //if (!joiner.IsJoined) return;
           
            Node.Reply(m, ring.Actors, cache.Items.ToArray());
            ring.Add(m.From);
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
            foreach (var kvp in cache.Database.Items)
            {
                var peers = ring.FindClosest(new DhtId(kvp.Key));
                var index = peers.IndexOf(n=> n == peer);
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
            foreach (var to in ring.FindClosest(new DhtId(key)))
                Node.Send(to, Box.Id, "AddLocal", key, value);            
            foreach (var subscription in ring.GetMatches(DhtOperation.Add, key))
                Node.Send(subscription.Actor, Box.Id, "SubscriptionMatched", "Add", key);
            //Node.Reply(mail, true);
        }

        void Get(Mail mail, string key)
        {
            var msgIds = new List<MessageId>();
            foreach (var to in ring.FindClosest(new DhtId(key)))            
                msgIds.Add(Node.Send(to, Box.Id, "GetLocal", key));            
            foreach (var subscription in ring.GetMatches(DhtOperation.Get, key))
                Node.Send(subscription.Actor, Box.Id, "SubscriptionMatched", "Get", key);
            var ids = msgIds.ToHashSet();
            var reply = Box.WaitFor(n=> ids.Contains(n.MessageId) && n.Args[0] != null, TimeSpan.FromSeconds(5));
            var result = reply.Coalesce(n => n.Args[0]) as byte[];          
            Node.Reply(mail, result);
        }


        void Remove(Mail mail, string key)
        {
            foreach (var to in ring.FindClosest(new DhtId(key)))
                Node.Send(to, Box.Id, "RemoveLocal", key);            
            foreach (var subscription in ring.GetMatches(DhtOperation.Remove, key))
                Node.Send(subscription.Actor, Box.Id, "SubscriptionMatched", "Remove", key);
           // Node.Reply(mail, true);
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
