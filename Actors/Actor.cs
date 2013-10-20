using System;
using Serialization;
using System.Threading;

namespace NetworkTransport
{
	public class Actor
	{
		public Actor (MailBox mail, MessageClient client, ISerializer serializer)
		{
			this.mailbox = mail;
			this.Id = Interlocked.Increment(ref this.Id);
			this.remote = client;
			this.serializer = serializer;
		}

		static int nextActorId;
		public int Id {get; private set;}
		MessageClient remote;
		MailBox mailbox;
		ISerializer serializer;
		int nextMailId =0;



		public int Send(string name, params object[] values){
			var message = new Message{
				ActorId = Id,
				MessageId = Interlocked.Increment(ref nextMailId),
				Args = values
			};

			var bytes = serializer.Serialize(message);
			remote.Send(bytes);
			return message.MessageId;
		}

		public object Receive(int messageId = 0){
			return mailbox.WaitFor(messageId);
		}
	}
}

