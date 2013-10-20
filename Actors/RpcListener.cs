using System;
using Serialization;
using System.Collections.Generic;

namespace NetworkTransport
{
	public class RpcListener : IDisposable
	{
		public RpcListener (string host, int port, ISerializer serializer)
		{
			this.listener = new Listener(host, port);
			this.listener.Accepted += HandleAccepted;
			this.serializer = serializer;
		}

		ISerializer serializer;
		Listener listener;
		Dictionary<string, Type> handlers = new Dictionary<string, Type>();

		public void Add<T>(){
			Add(typeof(T));
		}

		public void Add(Type t){
			handlers[t.Name] = t;
		}

		void HandleAccepted (System.Net.Sockets.TcpClient obj)
		{
			var reader = new MessageReader(obj);
			reader.MessageReceived += HandleMessageReceived;
		}

		void HandleMessageReceived (Message obj)
		{
			var rpc = serializer.Deserialize<object>(obj.Buffer);
		}

		public void Dispose(){
			listener.Dispose();
		}

	}
}

