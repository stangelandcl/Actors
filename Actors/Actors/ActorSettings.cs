using System;
using Serialization;

namespace Actors
{
	public class ActorSettings
	{
		public MessageClient Remote {get;set;}
		public MailBox MailBox {get; set;}
		public ISerializer Serializer {get;set;}
	}
}

