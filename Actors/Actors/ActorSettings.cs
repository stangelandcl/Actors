using System;
using Serialization;
using Actors.Connections.Bytes;
using Actors.Connections;

namespace Actors
{
	public class ActorSettings
	{
		public IConnection Remote {get;set;}
		public MailBox MailBox {get; set;}
		public ISerializer Serializer {get;set;}
	}
}

