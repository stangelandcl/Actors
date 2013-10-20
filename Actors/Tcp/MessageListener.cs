using System;
using System.Net.Sockets;

namespace Actors
{
	public class MessageListener
	{
		public MessageListener (Listener l)
		{
			listener = l;
			l.Accepted += HandleAccepted;
		}

		public static implicit operator MessageListener(TcpClient l){
			return new MessageListener(l);
		}

		public static implicit operator MessageListener(Listener l){
			return new MessageListener(l);
		}
		public static implicit operator Listener(MessageListener l){
			return l.listener;
		}

		public event Action<MessageConnection> Connected;

		void HandleAccepted (System.Net.Sockets.TcpClient obj)
		{
			if(Connected != null)
				Connected(new MessageConnection(obj));
		}
		Listener listener;
	}
}

