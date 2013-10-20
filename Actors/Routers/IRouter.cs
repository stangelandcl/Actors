using System;

namespace Actors
{
	public interface IRouter
	{
		ISender GetSender(string computer, string world);
		void Add(string world, MessageTConnection c);
		event Action<object> Received;
	}
}

