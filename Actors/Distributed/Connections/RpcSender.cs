using System;


namespace Cls.Actors
{
	public class RpcSender : ISingleRpcSender
	{
		public RpcSender (Node node, IActorId id)
		{
			this.id = id;
			this.node = node;
		}
		IActorId id;
		Node node;

		#region ISingleRpcSender implementation

		public void Send (IActorId to, string name, params object[] args)
		{
			node.Send(to, id, name, args);
		}

		public void Reply (IActorId to, IMessageId msg, string name, params object[] args)
		{
			node.Reply(to, id, msg, name, args);
		}

		#endregion
	}
}

