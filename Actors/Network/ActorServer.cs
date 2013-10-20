using System;
using Serialization;
using System.Collections.Generic;

namespace Actors
{
	public class ActorServer : IDisposable
	{
		public ActorServer (string host, int port, ISerializer serializer = null)
		{
			listen = new Listener(host, port);
			listen.Accepted += HandleAccepted;
			this.serializer = serializer ?? Defaults.Serializer;
		}
		Listener listen;
		ISerializer serializer;
		List<ActorChannel> channels = new List<ActorChannel>();

		void HandleAccepted (System.Net.Sockets.TcpClient obj)
		{
			var channel = new ActorChannel(obj, serializer);
			channels.Add(channel);
		}

		public void Dispose(){
			listen.Dispose();
		}
	}
}

