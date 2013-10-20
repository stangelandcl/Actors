using System;
using Serialization;
using System.Threading;
using System.Collections.Generic;

namespace Actors
{
	public abstract class Actor : IDisposable
	{	
		public Actor(MailBox box, IWorld world)		
		{
			this.MailBox = box;
			this.World = world;
			this.MailBox.Received += HandleReceived;
		}

		public MailBox MailBox {get; private set;}
		public IWorld World {get; private set;}

		protected virtual void HandleReceived(){}

		public void Dispose(){
			MailBox.Received -= HandleReceived;
			World.Remove(this.MailBox.Id);
		}
	}
}

