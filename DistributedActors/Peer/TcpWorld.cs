using System;
using System.Collections.Generic;
using Serialization;
using System.Collections.Concurrent;
using System.Linq;

namespace Actors
{
	public class TcpWorld 
	{
        Dictionary<string, DistributedActor> actors = new Dictionary<string, DistributedActor>();		

		public void Dispatch (Mail obj)
		{
            DistributedActor actor;
            lock (actors)
            {
                if (!actors.TryGetValue(obj.To.Name, out actor))
                    return;
                if (!IsMatch(obj.To, actor.Box.Id))
                    return;                     
            }
            actor.Box.Receive(obj);
		}

        private static bool IsMatch(ActorId id, ActorId actor)
        {
            return actor.Equals(id) ||
                   (id.IsLocal && id.Name == actor.Name);
        }
				
		public void Add(DistributedActor actor){
            lock (actors)
                actors[actor.Box.Id.Name] = actor;
		}

		public void Remove(ActorId id){
            lock (actors)
            {
                DistributedActor actor;
                if (!actors.TryGetValue(id.Name, out actor))
                    return;
                if (!IsMatch(id, actor.Box.Id))
                    return;
                actors.Remove(id.Name);
            }					
		}      
    }
}

