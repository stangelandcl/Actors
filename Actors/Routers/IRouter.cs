using System;

namespace Actors
{
	public interface IRouter
	{
		ISender GetSender(string computer, string world);
		void Add(string computer, string world, MessageTConnection c);
		event Action<object> Received;
		string[] Worlds {get;}
		void Add(string computer, string world);
		void Remove(string computer, string world);
		void Add(string url);
		void Remove(string url);
	}
}

