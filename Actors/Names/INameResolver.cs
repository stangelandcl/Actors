using System;

namespace Actors
{
	public interface INameResolver
	{ 
		ActorId NameToAddress(string name);
		void Alias(string name, ActorId address);
		void Add(INameResolver resolver);
		void Remove(INameResolver resolver);
	}
}

