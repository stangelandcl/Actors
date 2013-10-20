using System;
using System.Collections.Generic;
using Serialization;
using System.Collections.Concurrent;
using System.Linq;

namespace Actors
{
	public class TcpWorld 
	{
		public TcpWorld(ISerializer serializer)
		{
			this.defaultSerializer = serializer;
		}

		ISerializer defaultSerializer;
		Dictionary<ActorId, Actor> actors = new Dictionary<ActorId, Actor>();


		public void Dispatch (Mail obj)
		{
			Actor a;
			lock(actors)
				foreach(var entry in DnsAlias.Get(obj.To))
					if(actors.TryGetValue(entry, out a))
						a.MailBox.Receive(obj);
		}
				
		public void Add(Actor actor){
			lock(actors)
				actors.Add(actor.MailBox.Id, actor);
		}

		public void Remove(ActorId id){
			lock(actors)
				foreach(var actor in actors.ToArray()){
					if(actor.Value.MailBox.Id == id)
						actors.Remove(actor.Key);
				}		
		}
	}
}

