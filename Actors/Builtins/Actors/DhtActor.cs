using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Dht;

namespace Actors.Builtin.Actors
{
    public class DhtActor : Actor
    {
        public DhtActor(ILocalData cache, string shortname = "System.Dht")
            : base(shortname)
        {
            this.cache = cache;
            this.ring = new DhtRing(Box.Id);
        }

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
                args[0] = new DhtMetadata{Originator = meta.Originator, TimeToLive = i++};
                Array.Copy(m.Args, 1, args, 1, args.Length-1);
                Node.Send(to, Box.Id, m.Name, args);
            }
        }

        void JoinReply(Mail m, DhtMetadata meta, ActorId[] actors, KeyValuePair<object, object>[] data)
        {
            ring.AddRange(actors);
            cache.AddRange(data);
        }

        void Join(Mail m, DhtMetadata meta)
        {
            Node.Send(meta.Originator, Box.Id, "JoinReply", ring.Actors, cache.Data);
            var sendTo = ring.FindAllMax();
            int i = 0;
            foreach (var to in sendTo) 
                Node.Send(to, Box.Id, "PeerAdded", new DhtMetadata { Originator = Box.Id, TimeToLive = i++ });                       
        }

        void PeerAdded(Mail m, DhtMetadata meta, ActorId peer)
        {
            ring.Add(peer);
            Forward(m);
        }

        void PeerRemoved(Mail m, DhtMetadata meta, ActorId peer)
        {
            ring.Remove(peer);
            Forward(m);
        }

        void Add(Mail m, DhtMetadata meta, object key, object value)
        {
            cache[key] = value;
            Forward(m);
        }

        void Remove(Mail m, DhtMetadata meta, object key)
        {
            cache.Remove(key);
            Forward(m);
        }

        void Append(Mail m, DhtMetadata meta, object key, object value)
        {
            var x = cache[key];
            if (x == null)
                x = new List<object>();
            else if (!(x is List<object>))
                x = new List<object>(Enumerable.Repeat(x, 1));
            var list = (List<object>)x;
            list.Add(value);
            Forward(m);
        }

        void Remove(Mail m, DhtMetadata meta, object key, object value)
        {
            var x = cache[key];
            if (x == null) return;
            if (!(x is List<object>))
            {
                if (x.Equals(value))
                    cache.Remove(key);
            }
            else
            {
                var list = (List<object>)x;
                list.Remove(value);
                if (list.Count == 0)
                    cache.Remove(key);
            }
            Forward(m);
        }
    }
}
