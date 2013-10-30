using System;

namespace Actors
{
	public interface ISingleRpcSender
	{
		void Send(IActorId to, string name, params object[] args);
		void Reply(IActorId to, IMessageId msg, string name, params object[] args);
	}
	public interface IRpcSender
	{
		void Send(IActorId from, IActorId to, string name, params object[] args);
		void Reply(IActorId from, IActorId to, IMessageId msg, string name, params object[] args);
	}
}

