using System;
using Serialization;

namespace Actors
{
	public class EchoActor : Actor
	{
		public EchoActor (MailBox mail, Node node)
			: base(mail, node)
		{
			this.MailBox.Received += HandleReceived;
		}

		protected override void HandleReceived ()
		{
			var msg = MailBox.CheckFor(n=>n.Name == "echo");
			if(msg != null){
				Node.Reply(msg, "", "Server says: " + msg.Args[0]);
			}
		}
	}
}

