using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Actors.Extensions;

namespace Actors
{
    public struct Link
    {
        public ActorId Creator;
        public ActorId Other;
    }

    public enum LinkStatus
    {
        Create = 1,
        Died = 2,
        Disconnected = 3,
        Heartbeat = 4,       
    }

    public class LinkMap
    {
        Dictionary<ActorId, List<ActorId>> creatorToOther = new Dictionary<ActorId, List<ActorId>>();
        Dictionary<ActorId, List<ActorId>> otherToCreator = new Dictionary<ActorId, List<ActorId>>();
        public void Add(ActorId creator, ActorId other)
        {
            Add(new Link { Creator = creator, Other = other });
        }
        public void Add(Link link)
        {
            lock (creatorToOther)
            {
                creatorToOther.GetOrAdd(link.Creator).Add(link.Other);
                otherToCreator.GetOrAdd(link.Other).Add(link.Creator);
            }
        }

        public ActorId[] Get(ActorId id)
        {
            lock (creatorToOther)
            {
                var actors = new HashSet<ActorId>();
                List<ActorId> a;
                foreach (var alias in DnsAlias.Get(id))
                {
                    if (creatorToOther.TryGetValue(alias, out a))
                        actors.AddRange(a);
                    if (otherToCreator.TryGetValue(alias, out a))
                        actors.AddRange(a);
                }
                return actors.ToArray();
            }
        }

        public void Remove(Link link)
        {
            lock (creatorToOther)
            {
                List<ActorId> a;
                foreach (var alias in DnsAlias.Get(link.Creator))
                    if (creatorToOther.TryGetValue(alias, out a))
                    {
                        a.Remove(alias);
                        if (!a.Any())
                            creatorToOther.Remove(alias);
                    }
                foreach(var alias in DnsAlias.Get(link.Other))
                    if (otherToCreator.TryGetValue(alias, out a))
                    {
                        a.Remove(alias);
                        if (!a.Any())
                            otherToCreator.Remove(alias);
                    }
                
            }
        }
    }
}
