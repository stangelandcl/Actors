using System;

namespace Actors
{
	public interface IMailRouter
	{
		IMailSender GetSender(ActorId id);
		void Add(MessageTConnection c);
		event Action<Mail> Received;
	}
}

