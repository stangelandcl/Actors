using System;
using Serialization;

namespace Actors
{
	public class EchoActor : Actor
	{
		public EchoActor (string shortName)
			: base(shortName)
		{
			this.Box.Received += HandleReceived;
		}

		protected override void HandleReceived ()
		{
			var msg = Box.CheckFor(n=>n.Name == "echo");
			if(msg != null){
				Node.Reply(msg, "", "Server says: " + msg.Args[0]);
			}
		}
	}
}

