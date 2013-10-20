using System;

namespace Actors
{
	public interface IWorld{
		void Listen(string host ,int port);	
		ActorId Resolve(string name);
		void Connect(string host, int port);
		void Add(Actor actor);
		void Remove(ActorId id);
		IMailSender GetSender(ActorId id);
	}
}

