using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Actors.Dht
{
    class DhtRing
    {
        public DhtRing(ActorId self)
        {
            this.selfId = self;
            this.actors.Add(self);
        }

        ActorId selfId;
        List<ActorId> actors = new List<ActorId>();

        public ActorId[] Actors
        {
            get
            {
                lock (actors)
                    return actors.ToArray();
            }
        }

        public ActorId? Predecessor
        {
            get
            {
                lock (actors)
                {
                    var index = SelfIndex;
                    var start = index;
                    if(--index < 0) index = actors.Count-1;
                    if (index == start) return null;
                    return actors[index];
                }
            }
        }

        public ActorId? Successor
        {
            get
            {
                lock (actors)
                {
                    var index = SelfIndex;
                    var start = index;
                    if (++index == actors.Count) index = 0;
                    if (index == start) return null;
                    return actors[index];
                }
            }
        }

        
        public void AddRange(IEnumerable<ActorId> actors)
        {
            lock(this.actors)
                this.actors.AddRange(actors);
        }

        public void Add(ActorId id)
        {
            lock (actors)
            {
                var index = actors.BinarySearch(id);
                if (index >= 0) return;
                actors.Insert(~index, id);
            }
        }

        public void Remove(ActorId id)
        {
            lock(actors)
                actors.Remove(id);
        }

        public IDisposable Lock()
        {
            Monitor.Enter(actors);
            return Disposable.New(Monitor.Exit, actors);
        }

        public ActorId[] FindAll(int ttl, ActorId? originator = null)
        {
            lock (actors)
                return Enumerable.Range(0, ttl)
                    .Select(n => Find(n))
                    .Where(n => n.HasValue)
                    .Select(n => n.Value)
                    .ToArray();
        }

        public ActorId[] FindAllMax()
        {
            lock (actors)
                return FindAll(MaxTimeToLive);
        }

        public ActorId? Find(int ttl, ActorId? originator = null)
        {
            lock (actors)
            {
                if (ttl < 0) return null;
                int selfIndex = SelfIndex;
                int distance = (int)Math.Pow(2, ttl);
                if (distance >= actors.Count) return null;
                int endPoint = selfIndex + distance;
                if (endPoint >= actors.Count)
                    endPoint -= actors.Count;
                var destination = actors[endPoint];
                if (destination.Equals(selfId)) return null;
                if (!originator.HasValue) return null;
                var originatorIndex = actors.BinarySearch(originator.Value);
                if (IsBetween(originatorIndex, selfIndex, endPoint))
                {
                    // if the originator is between us and the endpoint it means
                    // if we send this message it will have wrapped completely around
                    // the ring. if we do that nodes will receive the same message twice
                    // so stop this message instead.
                    return null;
                }
                return destination;
            }
        }

        bool IsBetween(int check, int start, int end)
        {
            if (start < end) 
                return check >= start && check <= end;
            return check >= start || check <= end;
        }

        int SelfIndex
        {
            get
            {
                lock (actors)
                    return actors.BinarySearch(selfId);
            }
        }

        public int MaxTimeToLive
        {
            get
            {
                lock (actors)
                    return (int)Math.Log(actors.Count, 2);
            }
        }
    }
}
