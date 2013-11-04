using System;

namespace Cls.Actors
{
	public interface IWorld : IDisposable
	{
		string Name {get;}
		void Add(DistributedActor actor);
		void Remove(ActorId id);
		void Dispatch(IMail message);
	}
}

