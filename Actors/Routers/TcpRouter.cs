using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace Actors
{
	public class TcpRouter : IRouter
	{	
		HashSet<MessageTConnection> connections = new HashSet<MessageTConnection>();
		public event Action<object> Received;

		public void AddConnection(ActorId actorId, MessageTConnection c){
			connections[actorId] = c;
			c.Receiver.Received += HandleReceived;
		}

		void HandleReceived (object obj)
		{
			if(Received != null)
				Received(obj);
		}

		public ISender GetSender (string computer, string world)
		{
			MessageTConnection c;
			if(!connections.TryGetValue(id, out c))
				return null;
			return c.Sender;

		}
	}
}

