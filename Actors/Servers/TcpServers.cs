using System;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Actors
{
	public class TcpServers
	{
		List<Listener> listeners = new List<Listener>();

		public IDisposable Listen(string host, int port){
			var listener = new Listener(host, port);
			lock(listeners)
				listeners.Add(listener);
			listener.Accepted += HandleAccepted;	
			return new DisposableAction(()=> Remove(listener), listener);
		}

		public event Action<MessageConnection> Connected;

		public void Remove(Listener listener){
			listener.Accepted -= HandleAccepted;
			lock(listeners)
				listeners.Remove(listener);		
		}

		void HandleAccepted (TcpClient obj)
		{
			if(Connected != null)
				Connected(obj);
		}
	}
}

