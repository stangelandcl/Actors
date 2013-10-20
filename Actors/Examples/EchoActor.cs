using System;
using Serialization;

namespace Actors
{
	public class EchoActor : Actor
	{
		public EchoActor (MailBox mail, IWorld world)
			: base(mail, world)
		{
			this.MailBox.Received += HandleReceived;
		}

		protected override void HandleReceived ()
		{
			var msg = MailBox.CheckFor(n=>n.Name == "echo");
			if(msg != null){
				World.GetSender(msg.From).Reply(msg, "", "Server says: " + msg.Args[0]);
			}
		}
	}
}

