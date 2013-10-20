using System;

namespace Actors
{
	public interface IWorld : IDisposable
	{
		string Name {get;}
		void Add(Actor actor);
		void Remove(ActorId id);
		void Dispatch(Mail message);
	}
}

