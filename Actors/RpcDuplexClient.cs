using System;
using Serialization;
using System.Net.Sockets;
using System.Net;

namespace NetworkTransport
{
	public class RpcDuplexClient : IDisposable
	{
		public RpcDuplexClient(string host, int port, ISerializer serializer)
			: this(new RpcMessageReader(new MessageReader(new TcpClient(host, port)), serializer))
		{}			

		public RpcDuplexClient (RpcMessageReader reader)
		{
			this.Reader = reader;
		}

		public RpcMessageReader Reader {get; private set;}

		public void Dispose(){
			Reader.Reader.Client.Client.Close();
		}
	}
}

