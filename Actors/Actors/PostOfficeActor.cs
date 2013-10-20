using System;
using Serialization;
using System.Collections.Generic;
using System.Threading;

namespace Actors
{
	public class PostOfficeActor : Actor
	{
		public PostOfficeActor(MailBox m, MessageClient c, ISerializer s)
			: base(m, c, s)
		{}		

		protected Dictionary<string, int> actorNames = new Dictionary<string, int>();
		protected Dictionary<int, MailBox> mailboxes = new Dictionary<int, MailBox>();
		static int nextActorId = 256;
		public int NextActorId(){
			return Interlocked.Increment(ref nextActorId);
		}
		public Actor NewActor(){
			return NewActor(this.NextActorId());
		}

		public void Add(string name, MailBox m){
			lock(mailboxes){
				mailboxes.Add(m.Id, m);
				actorNames.Add(name, m.Id);
			}
		}

		internal int GetIdFromName(string name){
			lock(mailboxes)
				return actorNames[name];
		}
	}
}

