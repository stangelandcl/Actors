using System;

namespace Actors
{
	public interface IMailRouter
	{
		MessageTConnection Connect(string host);
		void Add(MessageTConnection c);
		void Remove(MessageTConnection c);
		event Action<Mail> Received;
	}
}

