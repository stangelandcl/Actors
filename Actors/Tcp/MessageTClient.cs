using System;
using Serialization;

namespace Actors
{
	public class MessageTClient 
	{
		public MessageTClient (MessageClient client, ISerializer serializer)
		{
			this.Client = client;
			this.Serializer = serializer;
		}
		public MessageClient Client {get; private set;}
		public ISerializer Serializer {get; private set;}

		public void Send<T>(T item){
			Client.Send(Serializer.Serialize(item));
		}
	}
}

