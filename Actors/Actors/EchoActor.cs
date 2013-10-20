using System;
using Serialization;

namespace Actors
{
	public class EchoActor : Actor
	{
		public EchoActor (MailBox mail, MessageClient client, ISerializer serializer)
			: base(mail, client, serializer)
		{
			this.MailBox.Received += HandleReceived;
		}

		void HandleReceived ()
		{
			var msg = CheckFor(n=>n.Name == "echo");
			if(msg != null)
				Reply(msg, "", "Server says: " + msg.Args[0]);
		}
	}
}

