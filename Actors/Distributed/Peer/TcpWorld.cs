using System;
using System.Collections.Generic;

using System.Collections.Concurrent;
using System.Linq;
using Cls.Extensions;
using Cls.Connections;
using Cls.Serialization;


namespace Cls.Actors
{
	public class TcpWorld 
	{
        Dictionary<string, DistributedActor> actors = new Dictionary<string, DistributedActor>();		

		public void Dispatch (IMail obj)
		{
            DistributedActor actor;
            lock (actors)
            {
                if (!actors.TryGetValue(obj.As<RpcMail>().To.As<ActorId>().Name, out actor))
                    return;
                if (!IsMatch(obj.As<RpcMail>().To.As<ActorId>(), actor.Id))
                    return;                     
            }
            actor.Post(obj.As<RpcMail>());
		}

        private static bool IsMatch(ActorId id, ActorId actor)
        {
            return actor.Equals(id) ||
                   (id.IsLocal && id.Name == actor.Name);
        }
				
		public void Add(DistributedActor actor){
            lock (actors)
                actors[actor.Id.Name] = actor;
		}

		public DistributedActor[] Actors{
			get{
				return actors.Select(n=>n.Value).ToArray();
			}
		}

		public DistributedActor Get(ActorId id){
				lock(actors)
					return actors.GetOrDefault(id.Name);
			}

		public void Remove(ActorId id){
            lock (actors)
            {
                DistributedActor actor;
                if (!actors.TryGetValue(id.Name, out actor))
                    return;
                if (!IsMatch(id, actor.Id))
                    return;
                actors.Remove(id.Name);
            }					
		}      
    }
}

