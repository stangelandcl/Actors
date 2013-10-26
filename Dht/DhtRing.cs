using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Dht;
using System.Security.Cryptography;

namespace Actors.Dht
{
    public class DhtRing
    {
        public DhtRing(DhtId self)
        {
            this.selfId = self;
            this.actors.Add(self);
        }

        DhtId selfId;
        List<DhtId> actors = new List<DhtId>();
        SHA1Managed sha = new SHA1Managed();

        
        public DhtId[] Actors
        {
            get
            {
                lock (actors)
                    return actors.ToArray();
            }
        }

        public DhtId? Predecessor
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

        public DhtId? Successor
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

        public DhtId[] FindClosest(DhtId key, float ratio = 1f, int minCount = 3, int maxCount = int.MaxValue)
        {
            var count = (int)Math.Ceiling(actors.Count * ratio);
            count = count.Bound(minCount, maxCount);
            return GetClosest(key, count);
        }

        DhtId[] GetClosest(DhtId key, int count)
        {
            var hash = new DhtId(sha.ComputeHash(key.Bytes));
            var index = this.actors.BinarySearch(hash);
            if (index < 0) index = ~index - 1;
            if (index < 0) index = actors.Count - 1;
            var array = new DhtId[count];
            for (int i = 0; i < array.Length; i++)
                array[i] = actors[(index + i) % actors.Count];
            return array;
        }


        public void AddRange(IEnumerable<DhtId> actors)
        {
            lock(this.actors)
                this.actors.AddRange(actors);
        }

        public void Add(DhtId id)
        {
            lock (actors)
            {
                var index = actors.BinarySearch(id);
                if (index >= 0) return;
                actors.Insert(~index, id);
            }
        }

        public void Remove(DhtId id)
        {
            lock(actors)
                actors.Remove(id);
        }

        public IDisposable Lock()
        {
            Monitor.Enter(actors);
            return Disposable.New(Monitor.Exit, actors);
        }

        public DhtId[] FindAll(int ttl, DhtId? originator = null)
        {
            lock (actors)
                return Enumerable.Range(0, ttl)
                    .Select(n => Find(n))
                    .Where(n => n.HasValue)
                    .Select(n => n.Value)
                    .ToArray();
        }

        public DhtId[] FindAllMax()
        {
            lock (actors)
                return FindAll(MaxTimeToLive);
        }

        public DhtId? Find(int ttl, DhtId? originator = null)
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
