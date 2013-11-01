using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using System.Diagnostics;


namespace Actors
{
    [DebuggerDisplay("Count={Actors.Length}")]
    class DhtRing
    {
        public DhtRing(IActorId self)
        {
            this.HashAlgorithm = new SHA1Managed();
            this.SelfId = self;
            this.actors.Add(KeyValuePair.New(HashAlgorithm.ComputeHash(self.ToString()), self));            
        }

        public IActorId SelfId { get; private set; }
        List<KeyValuePair<byte[], IActorId>> actors = new List<KeyValuePair<byte[], IActorId>>();
        public HashAlgorithm HashAlgorithm { get; private set; }

        public List<KeyValuePair<byte[],IActorId>> Actors
        {
            get
            {
                return actors;                
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


        public IActorId Find(int ttl, IActorId originator = null)
        {
            lock (actors)
            {
                if (ttl < 0) return null;
                int selfIndex = SelfIndex;
                if (selfIndex < 0) return null;
                int distance = (int)Math.Pow(2, ttl);
                if (distance >= actors.Count) return null;
                int endPoint = selfIndex + distance;
                if (endPoint >= actors.Count)
                    endPoint -= actors.Count;
                var destination = actors[endPoint];
                if (destination.Equals(SelfId)) return null;
                if (originator == null) originator = SelfId;
                var hash = HashAlgorithm.ComputeHash(originator.ToString());
                var originatorIndex = actors.BinarySearch(KeyValuePair.New(hash,(IActorId) null), n => n.Key);
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

        public IActorId[] FindAll(int ttl, IActorId originator = null)
        {
            if (ttl < 0) return new IActorId[0];
            lock (actors)
                return Enumerable.Range(0, ttl)
                    .Select(n => Find(n))
                    .Where(n => n != null)                   
                    .ToArray();
        }

        public IActorId[] FindAllMax()
        {
            lock (actors)
                return FindAll(MaxTimeToLive);
        }

        public IActorId Predecessor
        {
            get
            {
                lock (actors)
                {
                    var index = SelfIndex;
                    var start = index;
                    if(--index < 0) index = actors.Count-1;
                    if (index == start) return null;
                    return actors[index].Value;
                }
            }
        }

        public IActorId Successor
        {
            get
            {
                lock (actors)
                {
                    var index = SelfIndex;
                    var start = index;
                    if (++index == actors.Count) index = 0;
                    if (index == start) return null;
                    return actors[index].Value;
                }
            }
        }

		public IActorId[] FindClosest(string key, float ratio = .1f, int minCount = 3, int maxCount = int.MaxValue)
        {
            var count = (int)Math.Ceiling(actors.Count * ratio);
            count = count.Bound(minCount, maxCount);
            return GetClosest(key, count);
        }

		IActorId[] GetClosest(string key, int count)
        {
            var hash = HashAlgorithm.ComputeHash(key.ToString());
            var array = new IActorId[count];
            lock (actors)
            {
                var index = this.actors.BinarySearch(KeyValuePair.New(hash, (IActorId)null), n => n.Key);
                if (index < 0) index = ~index - 1;
                if (index < 0) index = actors.Count - 1;               
                for (int i = 0; i < array.Length; i++)
                    array[i] = actors[(index + i) % actors.Count].Value;
            }
            return array;
        }


        public void AddRange(IEnumerable<IActorId> actors)
        {
            foreach (var actor in actors)
                AddInternal(actor);                 
        }
        public void Add(IActorId id)
        {
            AddInternal(id);           
        }

        void AddInternal(IActorId id)
        {            
            var hash = HashAlgorithm.ComputeHash(id.ToString());
            lock (actors)
            {
                var index = actors.BinarySearch(KeyValuePair.New(hash, (IActorId)null), n => n.Key);
                if (index >= 0) return;
                actors.Insert(~index, KeyValuePair.New(hash, id));
            }               
        }

        public void Remove(IActorId id)
        {
            lock (actors)
            {
                actors.RemoveWhere(n => n.Value.Equals(id));
            }
        }

        public IDisposable Lock()
        {
            Monitor.Enter(actors);
            return Disposable.New(Monitor.Exit, actors);
        }

      
        public int SelfIndex
        {
            get
            {
                var hash = HashAlgorithm.ComputeHash(SelfId.ToString());
                lock (actors)                                   
                    return actors.BinarySearch(KeyValuePair.New(hash,(IActorId) null), n => n.Key);                
            }
        }
       
    }
}
