using System;
using Actors;
using Dht.Ring;

namespace Dht
{
	public interface IDht
	{		
		void Join(IActorId[] peers);
		object Get( IResource resource);
		T Get<T>(IResource resource);
		void Put(IResource resource, object o);		
		void Remove(IResource resource);
	}
}

