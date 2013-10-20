using System;
using Serialization;

namespace Actors
{
	public class MessageTListener
	{
		public MessageTListener (MessageListener l, ISerializer serializer)
		{
			this.listener = listener;
			this.serializer = serializer;
			listener.Connected += HandleConnected;
		}

		public event Action<MessageTConnection> Connected;

		public static implicit operator MessageListener(MessageTListener t){
			return t.listener;
		}

		void HandleConnected (MessageConnection obj)
		{
			if(Connected != null)
				Connected(new MessageTConnection(obj));
		}

		ISerializer serializer;
		MessageListener listener;
	}
}

