using System;
using Serialization;

namespace Actors
{
	public class MessageTReader
	{
		public MessageTReader (MessageReader reader, ISerializer serializer)
		{
			reader.MessageReceived += HandleMessageReceived;
			this.serializer = serializer;
		}

		public event Action<object> Received;
		ISerializer serializer;

		void HandleMessageReceived (Message obj)
		{
			if(Received != null)
				Received(serializer.Deserialize<object>(obj.Buffer));
		}
	}
}

