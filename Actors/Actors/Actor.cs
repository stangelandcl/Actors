using System;
using Serialization;
using System.Threading;
using System.Collections.Generic;

namespace Actors
{
	public abstract class Actor : IDisposable
	{	
		public Actor(MailBox box, Node env)
		{
			this.MailBox = box;
			this.Node = env;
			this.MailBox.Received += HandleReceived;
		}

		public MailBox MailBox {get; private set;}
		public Node Node {get; private set;}

		protected virtual void HandleReceived(){}

		public void Dispose(){
			MailBox.Received -= HandleReceived;
			Node.Remove(this);
		}
	}
}

