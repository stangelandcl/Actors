using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Dht;
using Actors.Builtin.Clients;

namespace Actors.Builtin.Actors
{
    public class DhtActor : Actor, IDht
    {
        public DhtActor(ILocalData cache, string shortname = "System.Dht")
            : base(shortname)
        {
            this.cache = cache;
            this.ring = new DhtRing(Box.Id);
            Loop();
        }

        static readonly int nodeCheckIntervalMs = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
        static readonly TimeSpan nodeCheckTime = TimeSpan.FromSeconds(5);
        ILocalData cache;
        DhtRing ring;

        void Forward(Mail m)
        {
            var meta = (DhtMetadata)m.Args[0];
            var ttl = meta.TimeToLive;
            var sendTo = ring.FindAll(meta.TimeToLive - 1, meta.Originator);
            int i = 0;
            foreach (var to in sendTo)
            {
                var args = new object[m.Args.Length];
                args[0] = new DhtMetadata { Originator = meta.Originator, TimeToLive = i++ };
                Array.Copy(m.Args, 1, args, 1, args.Length-1);
                Node.Send(to, Box.Id, m.Name, args);
            }
        }

        void JoinReply(Mail m, DhtMetadata meta, ActorId[] actors, KeyValuePair<object, List<object>>[] data)
        {
            ring.AddRange(actors);
            cache.AddRange(data);
        }

        void Join(Mail m, DhtMetadata meta)
        {
            Node.Send(meta.Originator, Box.Id, "JoinReply", ring.Actors, cache.Data);
            SendToRing("PeerAdded", m.From);          
        }
     
        void SendToRing(FunctionId name, params object[] args)
        {
            var sendTo = ring.FindAllMax();
            int i = 0;
            foreach (var to in sendTo)
            {
                var toArgs = new object[args.Length + 1];
                toArgs[0] = new DhtMetadata { Originator = Box.Id, TimeToLive = i++ };
                for(int j=1;j<toArgs.Length;j++)
                    toArgs[j] = args[j-1];
                Node.Send(to, Box.Id, name, toArgs);
            }
        }

        void PeerAdded(Mail m, DhtMetadata meta, ActorId peer)
        {
            ring.Add(peer);
            Forward(m);
        }

        void IsAlive(Mail m)
        {
            Node.Reply(m);
        }        

        void PeerRemoved(Mail m, DhtMetadata meta, ActorId peer)
        {
            ring.Remove(peer);
            Forward(m);
        }

        void Get(Mail m, object key)
        {            
            Node.Reply(m, cache.Get(key));
        }

        void Replace(Mail m, DhtMetadata meta, object key, object value)
        {
            cache.Replace(key, value);
            Forward(m);
        }

        void Remove(Mail m, DhtMetadata meta, object key)
        {
            cache.Remove(key);
            Forward(m);
        }

        void Append(Mail m, DhtMetadata meta, object key, object value)
        {
            cache.Append(key, value);
            Forward(m);
        }

        void Remove(Mail m, DhtMetadata meta, object key, object value)
        {
            cache.Remove(key, value);
            Forward(m);
        }


        private void Loop()
        {
            Run(CheckAlive, nodeCheckIntervalMs);
        }


        void CheckAlive()
        {
            try
            {
                var predecessor = ring.Predecessor;
                if (predecessor.HasValue)
                    CheckAlive(predecessor.Value);

                var successor = ring.Successor;
                if (successor.HasValue)
                    CheckAlive(successor.Value);
            }
            finally
            {
                Loop();
            }
        }

        private void CheckAlive(ActorId node)
        {
            var msg = Node.Send(node, Box.Id, "IsAlive");
            var mail = Box.WaitFor(msg, nodeCheckTime);
            if (mail == null)
                SendToRing("PeerRemoved", node);          
        }

        public List<object> Get(object key)
        {
            return cache.Get(key);
        }

        public void Replace(object key, object value)
        {
            SendToRing("Replace", key, value);
        }
        public void Remove(object key)
        {
            SendToRing("Remove", key);
        }

        public void Append(object key, object value)
        {
            SendToRing("Append", key, value);
        }

        public void Remove(object key, object value)
        {
            SendToRing("Remove", key, value);
        }
    }
}
