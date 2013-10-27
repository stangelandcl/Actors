using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors.Dht
{
    class DhtJoiner
    {
        public DhtJoiner(Actor actor, Action<Action, int> run)
        {
            this.actor = actor;
            this.Run = run;
            Run(Join, 100);
        }
        int sleepCount;
        const int Minimum = 3;
        Actor actor;
        Action<Action, int> Run;
        int joined;
        int count = 0;
        Queue<ActorId> peers = new Queue<ActorId>();

        public void AddRange(IEnumerable<ActorId> ids)
        {
            lock (peers)
                foreach(var id in ids)
                    if (!peers.Contains(id))
                    {
                        peers.Enqueue(id);
                        sleepCount = 0;
                    }
        }       

        public void Joined()
        {
            joined++;
        }

        void Join()
        {
            if (IsJoined) return;
            if (sleepCount > 0)
            {
                sleepCount--;
                Run(Join, 100);
                return;
            }

            lock(peers)
            if(peers.Any()){
                var peer = peers.Dequeue();
                actor.Node.Send(peer, actor.Box.Id, "DhtJoin");
                peers.Enqueue(peer);
                if (++count == peers.Count)
                {
                    sleepCount = 5000 / 100;
                    return;
                }                
            }
            Run(Join, 100);
        }

        public bool IsJoined
        {
            get
            {
                return joined > Minimum ||
                    (joined > 0 && joined == peers.Count);
            }
        }

    }
}
