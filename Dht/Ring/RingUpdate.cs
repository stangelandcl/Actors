using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Dht;
using Actors;

namespace Dht.Ring
{
    class RingUpdate
    {
        public RingUpdate(DhtRing ring)
        {
            this.ring = ring;
        }
        DhtRing ring;

        public IActorId[] FindAll(int ttl, IActorId? originator = null)
        {
            if (ttl < 0) return new IActorId[0];
            using(ring.Lock())
                return Enumerable.Range(0, ttl)
                    .Select(n => Find(n))
                    .Where(n => n.HasValue)
                    .Select(n => n.Value)
                    .ToArray();
        }

        public IActorId[] FindAllMax()
        {
            using(ring.Lock())
                return FindAll(MaxTimeToLive);
        }

        public IActorId? Find(int ttl, IActorId? originator = null)
        {
            using(ring.Lock())
            {
                if (ttl < 0) return null;
                int selfIndex = ring.SelfIndex;
                if (selfIndex < 0) return null;
                int distance = (int)Math.Pow(2, ttl);
                if (distance >= ring.Actors.Count) return null;
                int endPoint = selfIndex + distance;
                if (endPoint >= ring.Actors.Count)
                    endPoint -= ring.Actors.Count;
                var destination = ring.Actors[endPoint];
                if (destination.Equals(ring.SelfId)) return null;
                if (!originator.HasValue) originator = selfId;
                var hash = ring.HashAlgorithm.ComputeHash(originator.Value.ToString());
                var originatorIndex = ring.Actors.BinarySearch(KeyValuePair.New(hash, ActorId.Empty), n => n.Key);
                if (IsBetween(originatorIndex, selfIndex, endPoint))
                {
                    // if the originator is between us and the endpoint it means
                    // if we send this message it will have wrapped completely around
                    // the ring. if we do that nodes will receive the same message twice
                    // so stop this message instead.
                    return null;
                }
                return destination.Value;
            }
        }

        bool IsBetween(int check, int start, int end)
        {
            if (check == start) return false;
            if (start < end)
                return check >= start && check <= end;
            return check >= start || check <= end;
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
