using System;

namespace Actors
{
	public class RemoteActor : Actor
	{
		public RemoteActor (string shortName, ActorId remote)
			: base(shortName)
		{		
			this.Remote = remote;
		}
        public ActorId Remote { get; private set; }

		public Mail Receive(MessageId id){
			return Box.WaitFor(id);
		}

		public void Send (Mail mail)
		{
			Node.Send(mail);
		}

		public void Send (MessageId msg, FunctionId name, params object[] args)
		{
			Node.Send(new Mail{
				To = Remote,
				From = Box.Id,
				Args = args,
				MessageId = msg, 
				Name = name,
			});
		}

		public MessageId Send (FunctionId name, params object[] args)
		{
			return Node.Send(new Mail{
				To = Remote,
				From = Box.Id,
				Args = args,
				MessageId = MessageId.New(), 
				Name = name,
			});
		}

		public void Reply (Mail mail,  params object[] args)
		{
			Node.Reply(mail,args);
		}

	}
}

