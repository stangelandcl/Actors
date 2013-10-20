using System;
using Serialization;
using System.Threading;

namespace Actors
{
	public class Actor : IDisposable
	{	
		public Actor(ActorSettings settings)
			: this(settings.MailBox, settings.Remote, settings.Serializer)
		{
		}

		public Actor (MailBox mail, MessageClient client, ISerializer serializer)
		{
			this.MailBox = mail;
			this.remote = client;
			this.serializer = serializer;
		}
		protected MessageClient remote;
		public MailBox MailBox {get; private set;}
		protected ISerializer serializer;
		int nextMailId =0;

		protected virtual Actor NewActor(int actorId){
			return new Actor(new MailBox(actorId), remote, serializer);
		}

		public Mail SendReceive(int actorId, string name, params object[] values){
			int id = Send(actorId, name, values);
			return Receive(id);
		}

		public T SendReceive<T>(int actorId, string name, params object[] values){
			var mail = SendReceive(actorId, name, values);
			return (T)Convert.ChangeType(mail.Args[0], typeof(T));
		}

		public Tuple<T1,T2> SendReceive<T1,T2>(int actorId, string name, params object[] values){
			var mail = SendReceive(actorId, name, values);
			return Tuple.Create(
				(T1)(object)mail.Args[0], 
				(T2)(object)mail.Args[1]);
		}
		public Tuple<T1,T2,T3> SendReceive<T1,T2,T3>(int actorId, string name, params object[] values){
			var mail = SendReceive(actorId, name, values);
			return Tuple.Create(
				(T1)(object)mail.Args[0], 
				(T2)(object)mail.Args[1], 
				(T3)(object)mail.Args[2]);
		}

		public int Send(int actorId, int messageId, string name, params object[] values){
			var mail = new Mail{
				ReceiverId = actorId,
				SenderId = MailBox.Id,
				MessageId = messageId,
				Name = name,
				Args = values
			};

			var bytes = serializer.Serialize(mail);
			remote.Send(bytes);
			return mail.MessageId;
		}

		public void Reply(Mail other, string name, params object[] args){
			Send(other.SenderId, other.MessageId, name, args);
		}

		public int Send(int actorId, string name, params object[] values){
			var mail = new Mail{
				ReceiverId = actorId,
				SenderId = MailBox.Id,
				MessageId = Interlocked.Increment(ref nextMailId),
				Name = name,
				Args = values
			};

			var bytes = serializer.Serialize(mail);
			remote.Send(bytes);
			return mail.MessageId;
		}

		public Mail Receive(int messageId = 0){
			if(messageId == 0) 
				return MailBox.WaitForAny();
			return MailBox.WaitFor(messageId);
		}

		public Mail CheckFor(Func<Mail, bool> filter, int timeout = 0){
			return MailBox.CheckFor(filter, timeout);
		}

		public T Receive<T>(int messageId = 0){
			var m = Receive(messageId);
			if(typeof(T) == typeof(string))
				return (T)(object)m.Name;
			throw new NotImplementedException();
		}

		public void Dispose(){
			remote.Client.Close();
		}
	}
}

