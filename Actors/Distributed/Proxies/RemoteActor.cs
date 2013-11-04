using System;
using Cls.Extensions;
using Cls.Connections;
using Cls.Serialization;


namespace Cls.Actors
{
	public class RemoteActor : DistributedActor
	{
		public RemoteActor (string shortName, ActorId remote)
			: base(shortName)
		{		
			this.Remote = remote;
		}
        public ActorId Remote { get; private set; }
		
		public void Send (IMail mail)
		{
			Node.Send(mail);
		}

		public void Send (IMessageId msg, string name, params object[] args)
		{
			Node.Send(new RpcMail{
				To = Remote,
				From = Id,				
				MessageId = msg.As<MessageId>(), 
				Message = new FunctionCall(name, args),				
			});
		}

		public IMessageId Send (string name, params object[] args)
		{
			return Node.Send(new RpcMail{
				To = Remote,
				From = Id,				
				MessageId = MessageId.New(), 
				Message = new FunctionCall(name, args),				
			});
		}

		public void Reply (IMail mail,  params object[] args)
		{
			Node.Reply(mail,args);
		}

	}
}

