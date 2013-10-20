using System;
using Serialization;
using System.Reflection;

namespace Actors
{
	public class NewInstance : Actor
	{
		public NewInstance(int id, PostOfficeActor office, MessageClient client, ISerializer serializer)
			: base(new MailBox(id), client, serializer)
		{
			this.office = office;
			this.MailBox.Received += HandleReceived;
		}

		PostOfficeActor office;

		void HandleReceived(){
			var mail = CheckFor(n=> n.Name == "new");
			if(mail == null) return;
			var type = Assembly.GetExecutingAssembly().GetType((string)mail.Args[0]);
			var id = office.NextActorId();
			var args = new object[3 + mail.Args.Length-2];
			args[0] = new MailBox(id);
			args[1] = this.remote;
			args[2] = this.serializer;
			for(int i=0;i<mail.Args.Length-2;i++)
				args[3+i] = mail.Args[i+2];
			var name = (string)mail.Args[1];
			var actor = (Actor) Activator.CreateInstance(type, args);
			office.Add(name, actor.MailBox);
			Reply(mail, "newed", actor.MailBox.Id);
		}
	}
}

