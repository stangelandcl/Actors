using System;
using Serialization;
using System.Collections.Generic;
using System.Threading;

namespace Actors
{
	public class PostOfficeActor : Actor
	{
		public PostOfficeActor(MailBox m, IWorld world)
			: base(m,world)
		{}

		protected Dictionary<ActorId, MailBox> mailboxes = new Dictionary<ActorId, MailBox>();	
		public void Add(MailBox m){
			lock(mailboxes)
				mailboxes.Add(m.Id, m);			
		}
		public void Remove(MailBox box){
			lock(mailboxes)
				mailboxes.Remove(box.Id);
		}
	}
}

