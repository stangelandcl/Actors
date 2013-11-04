using System;
using System.Collections.Generic;

namespace Cls.Actors
{
	public class NameResolver : INameResolver
	{
		List<INameResolver> resolvers = new List<INameResolver>();
		Dictionary<string, ActorId> names = new Dictionary<string, ActorId>();

		public ActorId NameToAddress (string name)
		{
			ActorId id;
			if(names.TryGetValue(name, out id))
				return id;
			foreach(var resolver in resolvers)
			{
				id = resolver.NameToAddress(name);
				if(!id.IsEmpty)
					break;
			}
			return id;
		}

		public void Alias (string name, ActorId address)
		{
			names.Add(name, address);
		}

		public void Add (INameResolver resolver)
		{
			resolvers.Add(resolver);
		}

		public void Remove(INameResolver r){
			resolvers.Remove(r);
		}
	}
}

