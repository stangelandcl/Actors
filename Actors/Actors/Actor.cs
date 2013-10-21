using System;
using Serialization;
using System.Threading;
using System.Collections.Generic;

namespace Actors
{
	public abstract class Actor : IDisposable
	{	
		public Actor(string shortName)
			: this(new MailBox(shortName), null)
		{}

		public Actor(MailBox box, Node node)
		{
			Node = node;
			Box = box;
			Box.Received += HandleReceived;
		}

		public MailBox Box {get; internal set;}
		public Node Node {get; internal set;}

		protected virtual void HandleReceived(){}


		public virtual void Dispose(){
			Box.Received -= HandleReceived;
			Node.Remove(this);
		}
	}
}

