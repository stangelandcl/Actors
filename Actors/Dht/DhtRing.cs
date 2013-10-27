using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Dht;
using System.Security.Cryptography;
using KeyValueDatabase;

namespace Actors.Dht
{
    class DhtRing
    {
        public DhtRing(ActorId self, IDhtBackend db)
        {
            this.db = db;
            this.selfId = self;
            this.actors.Add(KeyValuePair.New(sha.ComputeHash(self.ToString()), self));
        }

        IDhtBackend db;
        ActorId selfId;
        List<KeyValuePair<byte[], ActorId>> actors = new List<KeyValuePair<byte[],ActorId>>();
        SHA1Managed sha = new SHA1Managed();
        List<Subscription> subscriptions = new List<Subscription>();

        public void Subscribe(ActorId node, DhtOperation operations, string keyRegex)
        {           
            var subscription = new Subscription(node, operations, keyRegex);
            Subscribe(subscription);                   
        }

        public void Subscribe(Subscription subscription)
        {
            lock (subscriptions)
                subscriptions.Add(subscription);
        }

		public void Unsubscribe(ActorId node, DhtOperation operations, string keyRegex)
        {
            var subscription = new Subscription(node, operations, keyRegex);
            Unsubscribe(subscription);
        }

        public void Unsubscribe(Subscription subscription)
        {
            lock (subscriptions)
                subscriptions.Remove(subscription);
        }

        public Subscription[] GetMatches(DhtOperation operation, string key)
        {
            lock (subscriptions)
                return subscriptions.Where(n => n.IsMatch(operation, key)).ToArray();
        }

        
        public ActorId[] Actors
        {
            get
            {
                lock (actors)
                    return actors.Select(n => n.Value).ToArray();
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
                    return actors[index].Value;
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
                    return actors[index].Value;
                }
            }
        }

        public ActorId[] FindClosest(DhtId key, float ratio = .1f, int minCount = 3, int maxCount = int.MaxValue)
        {
            var count = (int)Math.Ceiling(actors.Count * ratio);
            count = count.Bound(minCount, maxCount);
            return GetClosest(key, count);
        }

        ActorId[] GetClosest(DhtId key, int count)
        {
            var hash = sha.ComputeHash(key.Bytes);
            var array = new ActorId[count];
            lock (actors)
            {
                var index = this.actors.BinarySearch(KeyValuePair.New(hash, ActorId.Empty), n => n.Key);
                if (index < 0) index = ~index - 1;
                if (index < 0) index = actors.Count - 1;               
                for (int i = 0; i < array.Length; i++)
                    array[i] = actors[(index + i) % actors.Count].Value;
            }
            return array;
        }


        public void AddRange(IEnumerable<ActorId> actors)
        {
            foreach (var actor in actors)
                AddInternal(actor);
            db.Peers.AddRange(actors);         
        }
        public void Add(ActorId id)
        {
            AddInternal(id);
            db.Peers.AddRange(Enumerable.Repeat(id, 1));
        }

        void AddInternal(ActorId id)
        {            
            var hash = sha.ComputeHash(id.ToString());
            lock (actors)
            {
                var index = actors.BinarySearch(KeyValuePair.New(hash, ActorId.Empty), n => n.Key);
                if (index >= 0) return;
                actors.Insert(~index, KeyValuePair.New(hash, id));
            }               
        }

        public void Remove(ActorId id)
        {
            lock (actors)
            {
                actors.RemoveWhere(n=> n.Value == id);
                db.Peers.Remove(id);              
            }
        }

        public IDisposable Lock()
        {
            Monitor.Enter(actors);
            return Disposable.New(Monitor.Exit, actors);
        }

        public ActorId[] FindAll(int ttl, ActorId? originator = null)
        {
            if (ttl < 0) return new ActorId[0];
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
                if (selfIndex < 0) return null;
                int distance = (int)Math.Pow(2, ttl);
                if (distance >= actors.Count) return null;
                int endPoint = selfIndex + distance;
                if (endPoint >= actors.Count)
                    endPoint -= actors.Count;
                var destination = actors[endPoint];
                if (destination.Equals(selfId)) return null;
                if (!originator.HasValue) originator = selfId;
                var hash = sha.ComputeHash(originator.Value.ToString());
                var originatorIndex = actors.BinarySearch(KeyValuePair.New(hash, ActorId.Empty), n=>n.Key);
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
            if (start < end) 
                return check >= start && check <= end;
            return check >= start || check <= end;
        }

        int SelfIndex
        {
            get
            {
                var hash = sha.ComputeHash(selfId.ToString());
                lock (actors)                                   
                    return actors.BinarySearch(KeyValuePair.New(hash, ActorId.Empty), n => n.Key);                
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
