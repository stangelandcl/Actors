using System;


namespace Cls.Actors
{
	public interface IDht
	{		
		void Join(params IActorId[] peers);
		object Get( string resource);
		T Get<T>(string resource);
		void Put(string resource, object o);		
		void Remove(string resource);
	}
}

