using System;

namespace Cls.Actors
{
	public interface IMailSender
	{
		void Send(IMail mail);
		void Send(ActorId to, ActorId fromId, MessageId msg, string name, params object[] args);
		void Send(ActorId to, string name, params object[] args);
		void Reply(IMail mail, string name, params object[] args);
	}
}

