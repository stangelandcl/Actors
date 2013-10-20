using System;
using System.Net.Sockets;

namespace Actors
{
	public class TcpConnector
	{
		public TcpConnector ()
		{
		}

		public event Action<MessageConnection> Connected;

		public MessageConnection Connect(string host, int port){
			var msg = new MessageConnection(new TcpClient(host, port));
			if(Connected != null)
				Connected(msg);
			return msg;
		}
	}
}

