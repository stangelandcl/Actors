using System;

namespace Actors
{
	public interface IMailSender
	{
		void Send(Mail mail);
		void Send(ActorId to, ActorId fromId, MessageId msg, FunctionId name, params object[] args);
		void Send(ActorId to, FunctionId name, params object[] args);
		void Reply(Mail mail, FunctionId name, params object[] args);
	}
}

