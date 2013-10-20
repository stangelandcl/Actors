using System;
using Serialization;

namespace Actors
{
	public class RemoteActor : Actor
	{
		public RemoteActor (int remoteId,MailBox mail, MessageClient client, ISerializer serializer)
			: base(mail, client, serializer)
		{
			this.remoteId = remoteId;
		}

		int remoteId;

		public int Send(string name, params object[] args){
			return this.Send(remoteId, name, args);
		}

		public T SendReceive<T>(string name, params object[] args){
			var msg = Send(name, args);
			return Receive<T>(msg);
		}
	}
}

